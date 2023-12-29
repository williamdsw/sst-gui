using System;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class SlotItemPrefab : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Color whiteColor;
        [SerializeField] private Color blackColor;
        [SerializeField] private Color greenColor;
        [SerializeField] private Color redColor;

        private int currentSaveSlot;

        public void Init(OutputItem item, int saveSlot, ToggleGroup toggleGroup, UnityAction<bool, int, string> onToogleEvent)
        {
            try
            {
                label.text = item.Content.Trim();

                if (!item.IsValid)
                {
                    label.color = whiteColor;
                    backgroundImage.color = redColor;
                    return;
                }

                currentSaveSlot = saveSlot;
                toggle.group = toggleGroup;
                toggle.onValueChanged.AddListener(selected =>
                {
                    label.color = selected ? whiteColor : blackColor;
                    backgroundImage.color = selected ? greenColor : whiteColor;

                    if (!selected) return;

                    onToogleEvent.Invoke(item.IsValid, currentSaveSlot, item.Content.Trim());
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}