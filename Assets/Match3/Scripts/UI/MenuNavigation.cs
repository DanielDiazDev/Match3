using UnityEngine;

namespace UI
{
    public class MenuNavigation : MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _optionsPanel;
        [SerializeField] private GameObject _levelSelectorPanel;
        private void Start()
        {
            ShowMainMenu();
        }
        public void ShowMainMenu()
        {
            _mainMenuPanel.SetActive(true);
            _optionsPanel.SetActive(false);
            _levelSelectorPanel.SetActive(false);
        }

        public void ShowOptions()
        {
            _mainMenuPanel.SetActive(false);
            _optionsPanel.SetActive(true);
            _levelSelectorPanel.SetActive(false);
        }

        public void ShowLevelSelector()
        {
            _mainMenuPanel.SetActive(false);
            _optionsPanel.SetActive(false);
            _levelSelectorPanel.SetActive(true);
        }
    }
}