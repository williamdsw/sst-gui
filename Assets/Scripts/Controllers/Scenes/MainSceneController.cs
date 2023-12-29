using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Controllers.Scenes
{
    public class MainSceneController : MonoBehaviour
    {
        private bool databaseExists = false;

        private void Awake()
        {
            Debug.Log(Application.dataPath);
            Debug.Log(Application.consoleLogPath);
            Debug.Log(Application.temporaryCachePath);
            Debug.Log(Application.persistentDataPath);
            Debug.Log(Application.streamingAssetsPath);
            Debug.Log(Application.absoluteURL);
            Debug.Log(System.IO.Directory.GetCurrentDirectory());
            // Debug.Log(String.Join(" ", new List<string>() { "path", "-t", "n" }));

        }

        private IEnumerator Start()
        {
            yield return ExtractDatabase();
            // yield return ShowLogos();
        }


        private IEnumerator ExtractDatabase()
        {
            if (!FileManager.Exists(Properties.SstPath))
            {
                FileManager.Copy(Properties.SstStreamingAssetsPath, Properties.SstPath);
            }

            databaseExists = true;
            yield return null;
        }

        // private IEnumerator ShowLogos()
        // {
        //     yield return new WaitUntil(() => databaseExists);
        //     float length = logoImageAnimator.runtimeAnimatorController.animationClips[0].length;
        //     yield return new WaitForSecondsRealtime(length);
        //     yield return new WaitUntil(() => LocalizationController.Instance != null);
        //     LocalizationController.Instance.GetLocalization(currentLanguage);
        //     yield return new WaitUntil(() => LocalizationController.Instance.DictionaryCount > 0);
        //     SceneManagerController.CallScene(SceneManagerController.SceneNames.IntroMessage);
        // }
    }
}