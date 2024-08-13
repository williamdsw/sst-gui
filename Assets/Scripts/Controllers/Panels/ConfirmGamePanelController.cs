using Enums;
using Models;
using ScrollViews;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Panels
{
    public class ConfirmGamePanelController : MonoBehaviour
    {
        // || Inspector

        [Header("Required UI Elements from Panel")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI messageLabel;
        [SerializeField] private TextMeshProUGUI gameNameLabel;
        [SerializeField] private TextMeshProUGUI gameProductIdVersionLabel;
        [SerializeField] private Button noButton;
        [SerializeField] private Button yesButton;

        private void Awake()
        {
            AddListeners();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddListeners()
        {
            try
            {
                noButton.onClick.AddListener(() => panel.SetActive(false));
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
        /// <param name="game"></param>
        /// <param name="yesCallback"></param>
        public void Show(Game game, Action yesCallback)
        {
            try
            {
                panel.SetActive(true);
                gameNameLabel.text = string.Format("{0} ({1})", game.Name, game.AreaCode);
                gameProductIdVersionLabel.text = string.Format("{0} ({1})", game.ProductId, game.Version);
                yesButton.onClick.RemoveAllListeners();
                yesButton.onClick.AddListener(() =>
                {
                    panel.SetActive(false);
                    yesCallback();
                });
            }
            catch (Exception ex)
            {
                OutputScrollViewController.Instance.Add(string.Format("Error on show: {0}", ex.Message), OutputItemEnum.Error);
                throw ex;
            }
        }
    }
}