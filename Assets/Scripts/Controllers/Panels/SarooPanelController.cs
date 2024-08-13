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
using System.Text;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Panels
{
    public class SarooPanelController : MonoBehaviour
    {
        // || Inspector

        [Header("Required UI Elements from Panel")]
        [SerializeField] private Button loadFileButton;
        [SerializeField] private Button importGameButton;
        [SerializeField] private Transform scrollRectViewportContent;
        [SerializeField] private ToggleGroup scrollRectViewportContentToggleGroup;
        [SerializeField] private SlotItemPrefab slotItemPrefab;
        [SerializeField] private SarooSaveSlotPanelController sarooSaveSlotPanelController;
        [SerializeField] private ConfirmGamePanelController confirmGamePanelController;

        // || Cached

        private string filePath = string.Empty;
        private int currentSaveSlot = -1;
        private byte[] sarooContentFile;
        private SarooSaveItem currentSarooSaveItem = null;

        private List<SarooSaveItem> validSarooSaveItems;
        private List<OutputItem> fileOutputItems;
        private List<Game> games;

        // TODO
        private Language language = new Language();

        private void Awake()
        {
            fileOutputItems = new List<OutputItem>();
            validSarooSaveItems = new List<SarooSaveItem>();
            games = new List<Game>();

            LoadGamesFromJson();
            AddListeners();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadGamesFromJson()
        {
            try
            {
                string content = File.ReadAllText(Path.Join(Application.streamingAssetsPath, "games.json"));
                games = JsonConvert.DeserializeObject<List<Game>>(content);
            }
            catch (Exception ex)
            {
                OutputScrollViewController.Instance.Add(string.Format("Error on loading games from JSON file: {0}", ex.Message), OutputItemEnum.Error);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddListeners()
        {
            try
            {
                loadFileButton.onClick.AddListener(() =>
                {
                    importGameButton.interactable = false;
                    StandaloneFileBrowser.OpenFilePanelAsync(language.ChooseSarooFile, string.Empty, Properties.FixedStrings.Bin, false, OnSarooFileChooseCallback);
                });

                importGameButton.onClick.AddListener(() =>
                {
                    StandaloneFileBrowser.OpenFilePanelAsync(language.ChooseSarooFile, string.Empty, Properties.FixedStrings.Bin, false, OnGameChooseCallback);
                });
            }
            catch (Exception ex)
            {
                OutputScrollViewController.Instance.Add(string.Format("Error on add listeners: {0}", ex.Message), OutputItemEnum.Error);
                throw ex;
            }
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

                sarooSaveSlotPanelController.Reset();

                // TODO
                OutputScrollViewController.Instance.Add(string.Format("Importing {0} file", paths[0]), OutputItemEnum.Info);

                sarooContentFile = FileManager.Test(paths[0]);

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
            try
            {
                if (paths.Length == 0) return;

                string path = paths[0];
                string system = FileManager.GetSystemInFile(path);
                if (system.Trim() != "SEGA SEGASATURN")
                {
                    // TODO
                    string text = "Please choose a GAME file!";
                    OutputScrollViewController.Instance.Add(text, OutputItemEnum.Error);
                    return;
                }

                string content = FileManager.GetIdVersionInFile(path);

                // Validate if its a valid game by header

                if (string.IsNullOrEmpty(content))
                {
                    // TODO
                    string text = "Please choose a valid game!";
                    OutputScrollViewController.Instance.Add(text, OutputItemEnum.Error);
                    return;
                }

                if (validSarooSaveItems.FindIndex(item => item.RawHeader == content) != -1)
                {
                    // TODO
                    string text = string.Format("[{0}] already exists in SAROO Save File!", content);
                    OutputScrollViewController.Instance.Add(text, OutputItemEnum.Error);
                    return;
                }

                Debug.Log(content);

                string[] values = content.Split("V");
                string productId = values[0].Trim();
                string version = $"V{values[1].Trim()}";
                Game game = games.Find(item => item.ProductId == productId && item.Version == version) ?? new Game()
                {
                    Name = string.Empty,
                    ProductId = productId,
                    Version = version,
                    AreaCode = string.Empty
                };

                // TODO remove
                if (game == null)
                {
                    Debug.LogErrorFormat("{0} -> {1}", productId, version);
                }

                confirmGamePanelController.Show(game, () =>
                {
                    string data = validSarooSaveItems[^1].RawHeader;
                    FileManager.Test2(filePath.Replace("\"", ""), FileManager.Search(sarooContentFile, Encoding.UTF8.GetBytes(data)), Encoding.UTF8.GetBytes(content));

                    OnSarooFileChooseCallback(new string[] { filePath.Replace("\"", "") });
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerator ClearList()
        {
            validSarooSaveItems.Clear();

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

                    // Invalid content such as backup ram file, single save file, game file and others.
                    if (!IsSarooItemValid(item.Content))
                    {
                        item.Content = item.IsValid ? language.DoNotImportBackupRam : language.DoNotImportSaveFile;
                        item.IsValid = false;
                        SlotItemPrefab invalidSlotItem = Instantiate(slotItemPrefab, scrollRectViewportContent.transform.position, Quaternion.identity);
                        invalidSlotItem.transform.SetParent(scrollRectViewportContent);
                        invalidSlotItem.Init(item, index + 1, scrollRectViewportContentToggleGroup, null);
                        OutputScrollViewController.Instance.Add(item.Content, OutputItemEnum.Error);
                        yield break;
                    }

                    string text = item.Content;
                    bool containsSquareBrackets = text.Contains('[') && text.Contains(']');
                    bool hasValidHeader = false;

                    // Formatting invalid items e removing from list
                    if (!containsSquareBrackets)
                    {
                        if (index + 1 <= fileOutputItems.Count)
                        {
                            text += fileOutputItems[index + 1].Content.Trim();
                            fileOutputItems.RemoveAt(index + 1);
                        }
                        item.Content = text;
                        item.IsValid = false;
                    }

                    // Checking valid content
                    if (containsSquareBrackets)
                    {
                        string v = text[text.IndexOf('[')..].Replace("[", string.Empty).Replace("]", string.Empty);
                        int indexOfVersion = v.LastIndexOf("V");
                        item.IsValid = hasValidHeader = v.Length == 16 && indexOfVersion != -1;

                        if (hasValidHeader)
                        {
                            validSarooSaveItems.Add(new SarooSaveItem(v));
                        }
                    }

                    SlotItemPrefab validSlotItem = Instantiate(slotItemPrefab, scrollRectViewportContent.transform.position, Quaternion.identity);
                    validSlotItem.transform.SetParent(scrollRectViewportContent);
                    validSlotItem.Init(item, index + 1, scrollRectViewportContentToggleGroup, OnSlotItemChoose);
                }

                // TODO
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

        public void TriggerCurrentSlotItem() => OnSlotItemChoose(true, currentSaveSlot, currentSarooSaveItem.RawHeader);

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
                currentSarooSaveItem = validSarooSaveItems.Find(item => item.FormattedHeader == name[name.IndexOf("[")..].Trim());

                if (!isValid || currentSarooSaveItem == null)
                {
                    // TODO
                    OutputScrollViewController.Instance.Add(string.Format("{0} is an invalid game", name), OutputItemEnum.Error);
                    sarooSaveSlotPanelController.Reset();
                    return;
                }

                currentSaveSlot = save;

                // TODO
                OutputScrollViewController.Instance.Add(string.Format("Importing saves from {0}", currentSarooSaveItem.RawHeader), OutputItemEnum.Info);

                List<string> arguments = new() { filePath, "-t", currentSaveSlot.ToString() };
                OutputScrollViewController.Instance.Add(string.Format("sst.exe {0}", String.Join(" ", arguments)), OutputItemEnum.Info);
                List<OutputItem> message = ProcessHelper.Execute(arguments);

                if (message.Count == 0) return;
                if (message.Count == 1) message = new List<OutputItem>();
                if (message.Count > 1) message.RemoveAt(0);

                Game game = games.Find(item => item.ProductId == currentSarooSaveItem.ProductId && item.Version == currentSarooSaveItem.Version) ?? new Game()
                {
                    Name = "Invalid Game or Game Not Found!",
                    AreaCode = "NA",
                    Version = string.Empty,
                    ProductId = string.Empty,
                    TotalNumberOfSaves = 0
                };

                sarooSaveSlotPanelController.ShowList(filePath, currentSaveSlot, game, message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}