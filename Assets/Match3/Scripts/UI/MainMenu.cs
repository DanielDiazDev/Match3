using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _btnPlay;
        [SerializeField] private MenuNavigation _menuNavigation;

        private void Start()
        {
            _btnPlay.onClick.AddListener(PlayGame);
        }

        private void PlayGame()
        {
            _menuNavigation.ShowLevelSelector();
        }
    }
}