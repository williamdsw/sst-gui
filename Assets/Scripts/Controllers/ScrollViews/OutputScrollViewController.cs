using System;
using System.Collections;
using Enums;
using TMPro;
using UnityEngine;

namespace ScrollViews
{
    public class OutputScrollViewController : MonoBehaviour
    {
        [SerializeField] private Transform scrollViewContent;
        [SerializeField] private GameObject outputItemPrefab;
        [SerializeField] private Color infoColor;
        [SerializeField] private Color successColor;
        [SerializeField] private Color failColor;

        public static OutputScrollViewController Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator Clear()
        {
            foreach (Transform item in scrollViewContent)
            {
                Destroy(item.gameObject);
            }

            yield return new WaitUntil(() => scrollViewContent.childCount == 0);
        }

        public void Add(string text, OutputItemEnum status)
        {
            try
            {
                Debug.Log(text);
                GameObject item = Instantiate(outputItemPrefab, scrollViewContent.transform.position, Quaternion.identity);
                TextMeshProUGUI textMeshPro = item.GetComponent<TextMeshProUGUI>();
                textMeshPro.text = string.Format("{0} - {1}", DateTimeOffset.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss"), text);
                textMeshPro.color = status == OutputItemEnum.Success ? successColor : status == OutputItemEnum.Error ? failColor : infoColor;
                item.transform.SetParent(scrollViewContent);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }
}