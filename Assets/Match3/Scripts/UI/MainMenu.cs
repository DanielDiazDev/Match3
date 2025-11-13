using System;
using System.Sound;
using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _btnPlay;
        [SerializeField] private Button _btnOptions;
        [SerializeField] private Button _btnExit;
        [SerializeField] private MenuNavigation _menuNavigation;

        private void Start()
        {
            ServiceLocator.Instance.Get<SoundManager>().PlayMusic(SoundId.BackgroundMusic_1);
            _btnPlay.onClick.AddListener(PlayGame);
            _btnOptions.onClick.AddListener(GoToOptions);
            _btnExit.onClick.AddListener(ExitGame);
        }

        private void GoToOptions()
        {
            _menuNavigation.ShowOptions();
        }

        private void PlayGame()
        {
            _menuNavigation.ShowLevelSelector();
        }
        private void ExitGame()
        {
            Application.Quit();
        }
    }
}