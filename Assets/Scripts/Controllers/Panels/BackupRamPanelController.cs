using Enums;
using Helpers;
using JetBrains.Annotations;
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
    public class BackupRamPanelController : MonoBehaviour
    {
        [SerializeField] private Button loadFileButton;
        [SerializeField] private Button exportSelectedSaveButton;
        [SerializeField] private Transform scrollRectViewportContent;
        [SerializeField] private ToggleGroup scrollRectViewportContentToggleGroup;
        [SerializeField] private SlotItemPrefab slotItemPrefab;

        private List<OutputItem> fileOutputItems;

        private string filePath = string.Empty;
        [SerializeField] private int currentSaveSlot = -1;
        [SerializeField] private string currentSaveSlotName = string.Empty;
        [SerializeField] private string choosenFileName = string.Empty;

        [SerializeField] private byte[] other;
        [SerializeField] private byte[] globalTest = new byte[16];

        private Language language = new Language();

        private void Awake()
        {
            try
            {
                // using (var stream = new FileStream("C:\\Users\\William\\Downloads\\sst\\SS_SAVE.BIN", FileMode.Open, FileAccess.ReadWrite))
                // {
                //     for (int i = 0; i < 100; i++)
                //     {
                //         stream.Position = i;
                //         int b = stream.ReadByte();
                //         Debug.LogFormat("Position: {0}, Byte: {1}, Hex: {2}", i, b, b.ToString("X"));
                //     }

                //     other = new byte[stream.Length];
                //     stream.Read(other, 0, other.Length);
                //     Debug.Log(other.Length);

                //     for (int i = 0; i < other.Length; i++)
                //     {
                //         if (other[i] == 0)
                //         {
                //             for (int j = 0; j < 16; j++)
                //             {
                //                 Debug.LogFormat("i = {0}, j = {1}, i + j = {2}", i, j, (i + j));
                //                 if (i + j < other.Length)
                //                 {
                //                     byte next = other[i + j];
                //                     if (next == 0)
                //                     {
                //                         globalTest[j] = next;
                //                     }
                //                 }
                //             }
                //         }

                //         if (globalTest.Length == 16) break;
                //     }

                //     // stream.WriteByte(0x04);
                //     // Debug.Log(stream.ReadByte());
                //     // stream.Write(data, 0, data.Length);
                //     // stream.Position = 24;
                //     // stream.WriteByte(0x04);


                // }

                // using (var stream = new FileStream("c:\\Users\\William\\Downloads\\sst\\BKRAMSV.BIN", FileMode.Open, FileAccess.ReadWrite))
                // {
                //     List<byte> bytes = new List<byte>();

                //     for (int i = 0; i < 5000; i++)
                //     {
                //         stream.Position = i;
                //         int b = stream.ReadByte();
                //         Debug.LogFormat("Position: {0}, Byte: {1}, Hex: {2}", i, b, b.ToString("X"));
                //         bytes.Add((byte)b);
                //     }

                //     Debug.Log(System.Text.Encoding.UTF8.GetString(bytes.ToArray()));

                // stream.WriteByte(0x04);
                // Debug.Log(stream.ReadByte());
                // stream.Write(data, 0, data.Length);
                // stream.Position = 24;
                // stream.WriteByte(0x04);
                // }

                // byte[] test = new byte[16];
                // using (BinaryReader reader = new BinaryReader(new FileStream("L:\\roms\\saturn\\SAROO\\Andretti Racing (U)\\Andretti Racing (USA).bin", FileMode.Open)))
                // {
                //     reader.BaseStream.Seek(48, SeekOrigin.Begin);
                //     reader.Read(test, 0, 16);
                // }



                // foreach (var item in test)
                // {
                //     Debug.LogFormat("Byte: {0}, Hex: {1}", item, item.ToString("X"));
                // }

                // Debug.Log(System.Text.Encoding.UTF8.GetString(test));

                // string t = "T-5020H   V1.000";
                // Debug.Log(String.Join(",", test));
                // Debug.Log(String.Join(",", System.Text.Encoding.UTF8.GetBytes(t)));
            }
            catch (System.Exception)
            {

                throw;
            }


            fileOutputItems = new List<OutputItem>();

            loadFileButton.onClick.AddListener(() =>
            {
                StandaloneFileBrowser.OpenFilePanelAsync(language.ChooseSaturnRamFile, string.Empty, Properties.FixedStrings.Bin, false, OnBackupFileChooseCallback);
            });
            exportSelectedSaveButton.onClick.AddListener(() =>
            {
                try
                {
                    Debug.Log(filePath);
                    Debug.Log(FileManager.GetPathSubstring(filePath));

                    // string a = String.Copy(filePath);
                    // if (a.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
                    // {
                    //     foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    //     {
                    //         a = a.Replace(c.ToString(), string.Empty);
                    //     }
                    // }
                    // string b = String.Copy(FileManager.GetPathSubstring(filePath));
                    // if (b.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
                    // {
                    //     foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    //     {
                    //         b = b.Replace(c.ToString(), string.Empty);
                    //     }
                    // }

                    // Debug.Log(a);
                    // Debug.Log(b);


                    // Debug.Log(Path.GetFullPath(filePath));
                    // Debug.Log(Path.GetFullPath(FileManager.GetPathSubstring(filePath)));
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.Message);
                    Debug.Log(JsonConvert.SerializeObject(ex.Data));
                    Debug.Log(ex.Source);
                    Debug.Log(ex.StackTrace);
                    throw ex;
                }


                // StandaloneFileBrowser.SaveFilePanelAsync(language.SaveLocation, FileManager.GetPathSubstring(filePath) , choosenFileName, "bin", OnSaveFileChooseCallback);
                StandaloneFileBrowser.SaveFilePanelAsync(language.SaveLocation, string.Empty, choosenFileName, "bin", OnSaveFileChooseCallback);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths"></param>
        private void OnBackupFileChooseCallback(string[] paths)
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
        private void OnSaveFileChooseCallback(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrWhiteSpace(path))
                {
                    path = string.Format("\"{0}\"", path);

                    List<string> arguments = new() { filePath, "-s", currentSaveSlot.ToString() };
                    OutputScrollViewController.Instance.Add(string.Format("sst.exe {0}", String.Join(" ", arguments)), OutputItemEnum.Info);

                    List<OutputItem> output = ProcessHelper.Execute(arguments);
                    foreach (var item in output)
                    {
                        OutputScrollViewController.Instance.Add(string.Format("sst output: {0}", item.Content), OutputItemEnum.Info);
                    }

                    // string finalPath = Path.Combine(Application.persistentDataPath, currentSaveSlotName);
                    // string finalPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), currentSaveSlotName);
                    string finalPath = FileManager.GetSlotFinalPath(currentSaveSlotName);
                    if (FileManager.Exists(finalPath) && FileManager.Move(finalPath, path))
                    {
                        string folder = FileManager.GetFolder(path);

                        // TODO
                        OutputScrollViewController.Instance.Add(string.Format("Moving {0} to {1}", currentSaveSlotName, path), OutputItemEnum.Info);
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

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator ClearList()
        {
            foreach (Transform item in scrollRectViewportContent)
            {
                Destroy(item.gameObject);
            }

            yield return new WaitUntil(() => scrollRectViewportContent.childCount == 0);
            yield return PopulateList();
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator PopulateList()
        {
            int index = 0;
            foreach (OutputItem item in fileOutputItems)
            {
                try
                {
                    if (!IsBackupRamItemValid(item.Content))
                    {
                        item.Content = item.IsValid ? language.DoNotImportSarooFile : language.DoNotImportSaveFile;
                        item.IsValid = false;
                        SlotItemPrefab invalidSlotItem = Instantiate(slotItemPrefab, scrollRectViewportContent.transform.position, Quaternion.identity);
                        invalidSlotItem.transform.SetParent(scrollRectViewportContent);
                        invalidSlotItem.Init(item, index, scrollRectViewportContentToggleGroup, null);
                        OutputScrollViewController.Instance.Add(item.Content, OutputItemEnum.Error);
                        yield break;
                    }

                    SlotItemPrefab validSlotItem = Instantiate(slotItemPrefab, scrollRectViewportContent.transform.position, Quaternion.identity);
                    validSlotItem.transform.SetParent(scrollRectViewportContent);
                    validSlotItem.Init(item, index, scrollRectViewportContentToggleGroup, (isValid, saveSlot, slotName) =>
                    {
                        currentSaveSlot = saveSlot;
                        choosenFileName = slotName[slotName.IndexOf(" ")..].Trim();
                        currentSaveSlotName = string.Concat(choosenFileName, ".", Properties.FixedStrings.Bin);
                        exportSelectedSaveButton.interactable = true;
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                index++;
            }

            // TODO
            OutputScrollViewController.Instance.Add("Imported Backup RAM file with success!", OutputItemEnum.Success);

            yield return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private bool IsBackupRamItemValid(string item) => item.Contains(Properties.FixedStrings.Save) && item.IndexOf(Properties.FixedStrings.Save) == 0;
    }
}