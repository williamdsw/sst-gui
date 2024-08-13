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
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Panels
{
    public class SarooSaveSlotPanelController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameRegionLabel;
        [SerializeField] private Transform scrollRectViewportContent;
        [SerializeField] private ToggleGroup scrollRectViewportContentToggleGroup;
        [SerializeField] private Button importNewSlotButton;
        [SerializeField] private Button importSelectedSaveButton;
        [SerializeField] private Button exportSelectedSaveButton;
        [SerializeField] private SlotItemPrefab slotItemPrefab;
        [SerializeField] private SarooPanelController sarooPanelController;

        [SerializeField] private List<OutputItem> fileOutputItems;

        [SerializeField] private string currentFilePath = string.Empty;
        [SerializeField] private int currentSlot = -1;
        [SerializeField] private int currentSave = -1;
        [SerializeField] private string currentIdVersion = string.Empty;
        [SerializeField] private string currentSaveName = string.Empty;
        [SerializeField] private string choosenFileName = string.Empty;
        [SerializeField] private int numberOfSlots = 0;


        private void Awake()
        {
            fileOutputItems = new List<OutputItem>();

            importNewSlotButton.onClick.AddListener(() =>
            {
                // TODO
                StandaloneFileBrowser.OpenFilePanelAsync("Choose GAME save file", string.Empty, "bin", false, OnImportNewSlotCallback);
            });

            importSelectedSaveButton.onClick.AddListener(() =>
            {
                // TODO
                StandaloneFileBrowser.OpenFilePanelAsync("Choose GAME save file", string.Empty, "bin", false, OnImportGameIntoSelectedSlotCallback);
            });

            exportSelectedSaveButton.onClick.AddListener(() =>
            {
                // StandaloneFileBrowser.SaveFilePanelAsync("Save location", currentFilePath.Substring(0, currentFilePath.LastIndexOf("\\")), choosenFileName, "bin", (value) =>
                StandaloneFileBrowser.SaveFilePanelAsync("Save location", string.Empty, choosenFileName, "bin", OnExportSelectedGameCallback);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths"></param>
        private void OnImportNewSlotCallback(string[] paths)
        {
            try
            {
                if (paths.Length == 0) return;

                List<string> arguments = new() { currentFilePath, "-t", currentSlot.ToString(), "-i", string.Format("\"{0}\"", paths[0]) };

                // TODO
                OutputScrollViewController.Instance.Add(string.Format("sst.exe {0}", String.Join(" ", arguments)), OutputItemEnum.Info);

                List<OutputItem> output = ProcessHelper.Execute(arguments);
                Debug.Log(JsonConvert.SerializeObject(output));

                bool hasInvalidItem = output.Exists(item => !item.IsValid);

                foreach (var item in output)
                {
                    if (!item.IsValid)
                    {
                        OutputScrollViewController.Instance.Add(string.Format("sst.exe output: {0}", item.Content), OutputItemEnum.Error);
                        return;
                    }

                    OutputScrollViewController.Instance.Add(string.Format("sst.exe output: {0}", item.Content), hasInvalidItem ? OutputItemEnum.Error : OutputItemEnum.Success);
                }

                sarooPanelController.TriggerCurrentSlotItem();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths"></param>
        private void OnImportGameIntoSelectedSlotCallback(string[] paths)
        {
            try
            {
                if (paths.Length == 0) return;

                List<string> arguments = new() { currentFilePath, "-t", currentSlot.ToString(), "-s", currentSave.ToString(), "-i", string.Format("\"{0}\"", paths[0]) };

                // TODO
                OutputScrollViewController.Instance.Add(string.Format("sst.exe {0}", String.Join(" ", arguments)), OutputItemEnum.Info);

                List<OutputItem> output = ProcessHelper.Execute(arguments);
                Debug.Log(JsonConvert.SerializeObject(output));

                foreach (var item in output)
                {
                    if (!item.IsValid)
                    {
                        OutputScrollViewController.Instance.Add(string.Format("sst.exe output: {0}", item.Content), OutputItemEnum.Error);
                        return;
                    }

                    OutputScrollViewController.Instance.Add(string.Format("sst.exe output: {0}", item.Content), OutputItemEnum.Success);
                }

                sarooPanelController.TriggerCurrentSlotItem();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths"></param>
        private void OnExportSelectedGameCallback(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path))
                {
                    path = string.Format("\"{0}\"", path);

                    List<string> arguments = new() { currentFilePath, "-t", currentSlot.ToString(), "-s", currentSave.ToString() };
                    OutputScrollViewController.Instance.Add(string.Format("sst.exe {0}", String.Join(" ", arguments)), OutputItemEnum.Info);

                    List<OutputItem> output = ProcessHelper.Execute(arguments);
                    foreach (var item in output)
                    {
                        OutputScrollViewController.Instance.Add(string.Format("sst output: {0}", item.Content), OutputItemEnum.Info);
                    }

                    // string finalPath = Path.Combine(Application.persistentDataPath, currentSaveSlotName);
                    // string finalPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), currentSaveSlotName);
                    string finalPath = FileManager.GetSlotFinalPath(currentSaveName);
                    Debug.Log(finalPath);
                    if (FileManager.Exists(finalPath) && FileManager.Move(finalPath, path))
                    {
                        string folder = FileManager.GetFolder(path);

                        // TODO
                        OutputScrollViewController.Instance.Add(string.Format("Moving {0} to {1}", currentSaveName, path), OutputItemEnum.Info);
                        OutputScrollViewController.Instance.Add(string.Format("File {0} exported with success, opening folder", folder), OutputItemEnum.Success);
                        Application.OpenURL(folder);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Reset()
        {
            Debug.Log("Reset");
            importNewSlotButton.interactable = false;
            importSelectedSaveButton.interactable = exportSelectedSaveButton.interactable = false;
            StartCoroutine(ClearList(null));
        }

        public void ShowList(string filePath, int selectedSlot, Game game, List<OutputItem> saveSlotItems)
        {
            fileOutputItems = new List<OutputItem>(saveSlotItems);
            currentFilePath = filePath;
            currentSlot = selectedSlot;
            nameRegionLabel.text = string.Format("{0} ({1})", game.Name, game.AreaCode);
            importNewSlotButton.interactable = !string.IsNullOrEmpty(game.ProductId);
            importSelectedSaveButton.interactable = exportSelectedSaveButton.interactable = false;
            numberOfSlots = saveSlotItems.Count;
            StartCoroutine(ClearList(saveSlotItems));
        }

        private IEnumerator ClearList(List<OutputItem> saveSlotItems)
        {
            foreach (Transform item in scrollRectViewportContent)
            {
                Destroy(item.gameObject);
            }

            yield return new WaitUntil(() => scrollRectViewportContent.childCount == 0);

            if (saveSlotItems != null)
            {
                yield return PopulateList(saveSlotItems);
            }
        }

        private IEnumerator PopulateList(List<OutputItem> saveSlotItems)
        {
            int index = 0;
            foreach (OutputItem item in saveSlotItems)
            {
                if (!item.IsValid)
                {
                    importNewSlotButton.interactable = false;
                }

                SlotItemPrefab v = Instantiate(slotItemPrefab, scrollRectViewportContent.transform.position, Quaternion.identity);
                v.transform.SetParent(scrollRectViewportContent);
                v.Init(item, index, scrollRectViewportContentToggleGroup, (isValid, saveSlot, slotName) =>
                {
                    Debug.Log(slotName);
                    Debug.Log(slotName.IndexOf(":"));

                    currentSave = saveSlot;
                    choosenFileName = slotName[(slotName.IndexOf(":") + 1)..].Trim();
                    currentSaveName = string.Concat(choosenFileName, ".bin");
                    UnityEngine.Debug.LogFormat("currentSaveSlot : {0}", saveSlot);
                    UnityEngine.Debug.LogFormat("choosenFileName : {0}", choosenFileName);
                    UnityEngine.Debug.LogFormat("currentSaveSlotName : {0}", currentSaveName);
                    importSelectedSaveButton.interactable = exportSelectedSaveButton.interactable = true;
                });
                index++;
            }

            yield return null;
        }
    }
}