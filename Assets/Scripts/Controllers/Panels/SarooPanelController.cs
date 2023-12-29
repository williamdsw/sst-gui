using Enums;
using Helpers;
using Managers;
using Models;
using Newtonsoft.Json;
using ScrollViews;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Panels
{
    public class SarooPanelController : MonoBehaviour
    {
        [SerializeField] private Button loadFileButton;
        [SerializeField] private Button importGameButton;
        [SerializeField] private Transform scrollRectViewportContent;
        [SerializeField] private ToggleGroup scrollRectViewportContentToggleGroup;
        [SerializeField] private SlotItemPrefab slotItemPrefab;
        [SerializeField] private SarooSaveSlotPanelController sarooSaveSlotPanelController;

        [SerializeField] private List<string> validGames;

        private List<OutputItem> fileOutputItems;

        [SerializeField] private string filePath = string.Empty;
        [SerializeField] private int currentSaveSlot = -1;
        [SerializeField] private string currentSaveName = string.Empty;
        [SerializeField] private string currentSaveSlotName = string.Empty;
        [SerializeField] private string choosenFileName = string.Empty;

        private Language language = new Language();


        private void Awake()
        {
            fileOutputItems = new List<OutputItem>();
            validGames = new List<string>();

            loadFileButton.onClick.AddListener(() =>
            {
                importGameButton.interactable = false;
                StandaloneFileBrowser.OpenFilePanelAsync(language.ChooseSarooFile, string.Empty, Properties.FixedStrings.Bin, false, OnSarooFileChooseCallback);
            });

            importGameButton.onClick.AddListener(() =>
            {
                // TODO
                StandaloneFileBrowser.OpenFilePanelAsync(language.ChooseSarooFile, string.Empty, Properties.FixedStrings.Bin, false, OnGameChooseCallback);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths"></param>
        private void OnSarooFileChooseCallback(string[] paths)
        {
            try
            {
                if (paths.Length == 0) return;

                // TODO
                OutputScrollViewController.Instance.Add(string.Format("Importing {0} file", paths[0]), OutputItemEnum.Info);

                filePath = string.Format("\"{0}\"", paths[0]);
                List<string> arguments = new() { filePath };

                // TODO
                OutputScrollViewController.Instance.Add(string.Format("sst.exe {0}", String.Join(" ", arguments)), OutputItemEnum.Info);

                fileOutputItems = ProcessHelper.Execute(arguments);
                StartCoroutine(ClearList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void OnGameChooseCallback(string[] paths)
        {
            // TODO
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerator ClearList()
        {
            validGames.Clear();

            foreach (Transform item in scrollRectViewportContent)
            {
                Destroy(item.gameObject);
            }

            yield return new WaitUntil(() => scrollRectViewportContent.childCount == 0);
            yield return PopulateList();
        }

        private IEnumerator PopulateList()
        {

            try
            {
                for (int index = 0; index < fileOutputItems.Count; index++)
                {
                    OutputItem item = fileOutputItems[index];

                    if (!IsSarooItemValid(item.Content))
                    {
                        item.Content = item.IsValid ? language.DoNotImportBackupRam : language.DoNotImportSaveFile;
                        item.IsValid = false;
                        SlotItemPrefab invalidSlotItem = Instantiate(slotItemPrefab, scrollRectViewportContent.transform.position, Quaternion.identity);
                        invalidSlotItem.transform.SetParent(scrollRectViewportContent);
                        invalidSlotItem.Init(item, index, scrollRectViewportContentToggleGroup, null);
                        OutputScrollViewController.Instance.Add(item.Content, OutputItemEnum.Error);
                        yield break;
                    }

                    string text = item.Content;
                    bool containsSquareBrackets = text.Contains('[') && text.Contains(']');
                    bool hasValidHeader = false;

                    if (!containsSquareBrackets)
                    {
                        Debug.LogError(text);
                        if (index + 1 <= fileOutputItems.Count)
                        {
                            text += fileOutputItems[index + 1].Content.Trim();
                            fileOutputItems.RemoveAt(index + 1);
                        }
                        Debug.LogError(text);
                        item.Content = text;
                        item.IsValid = false;
                    }

                    if (containsSquareBrackets)
                    {
                        string v = text[text.IndexOf('[')..].Replace("[", string.Empty).Replace("]", string.Empty);
                        item.IsValid = hasValidHeader = v.Length == 16;

                        if (hasValidHeader)
                        {
                            validGames.Add(v);
                        }
                    }

                    SlotItemPrefab validSlotItem = Instantiate(slotItemPrefab, scrollRectViewportContent.transform.position, Quaternion.identity);
                    validSlotItem.transform.SetParent(scrollRectViewportContent);
                    validSlotItem.Init(item, index, scrollRectViewportContentToggleGroup, OnSlotItemChoose);
                }

                string a = fileOutputItems.Count >= 1 ? string.Format("{0} has {1} games", filePath, fileOutputItems.Count) : string.Format("{0} doesn't have any game", filePath);
                OutputScrollViewController.Instance.Add(a, OutputItemEnum.Info);
                importGameButton.interactable = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            yield return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private bool IsSarooItemValid(string item) => item.Contains(Properties.FixedStrings.Slot) && item.IndexOf(Properties.FixedStrings.Slot) == 0;

        public void TriggerCurrentSlotItem()
        {
            Debug.Log("TriggerCurrentSlotItem");
            Debug.Log(currentSaveSlot);
            Debug.Log(currentSaveName);
            OnSlotItemChoose(true, currentSaveSlot, currentSaveName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="save"></param>
        /// <param name="name"></param>
        private void OnSlotItemChoose(bool isValid, int save, string name)
        {
            try
            {
                if (!isValid)
                {
                    OutputScrollViewController.Instance.Add(string.Format("{0} is an invalid game", name), OutputItemEnum.Error);
                    sarooSaveSlotPanelController.Reset();
                    return;
                }

                currentSaveSlot = save;
                currentSaveName = name;
                choosenFileName = name[name.IndexOf("[")..].Trim();
                currentSaveSlotName = string.Concat(choosenFileName, ".", Properties.FixedStrings.Bin);

                // TODO
                OutputScrollViewController.Instance.Add(string.Format("Importing saves from {0}", choosenFileName), OutputItemEnum.Info);

                List<string> arguments = new() { filePath, "-t", currentSaveSlot.ToString() };
                OutputScrollViewController.Instance.Add(string.Format("sst.exe {0}", String.Join(" ", arguments)), OutputItemEnum.Info);
                List<OutputItem> message = ProcessHelper.Execute(arguments);
                Debug.Log(JsonConvert.SerializeObject(message));


                // Filter list
                // Get first item as id-version
                // Get others items as slots

                if (message.Count == 0)
                {
                    return;
                }

                if (message.Count == 1)
                {
                    message = new List<OutputItem>();
                }

                if (message.Count > 1)
                {
                    message.RemoveAt(0);
                }


                sarooSaveSlotPanelController.ShowList(filePath, currentSaveSlot, choosenFileName, message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}