using System;
using System.Sound;
using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Options : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [Header("Others")]
        [SerializeField] private Button _btnGoMenu;
        [SerializeField] private MenuNavigation _menuNavigation;
        [SerializeField] private Toggle _fullscreenToggle;

        private SoundManager _soundManager;


        private void Start()
        {
            _btnGoMenu.onClick.AddListener(GoToMainMenu);
            _soundManager = ServiceLocator.Instance.Get<SoundManager>();
            var fullScreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            _fullscreenToggle.isOn = fullScreen;
            Screen.fullScreen = fullScreen;
            _fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
            InitSlider(_masterSlider, "MasterVolume", _soundManager.SetMasterVolume);
            InitSlider(_musicSlider, "MusicVolume", _soundManager.SetMusicVolume);
            InitSlider(_sfxSlider, "SFXVolume", _soundManager.SetSFXVolume);
        }

        private void InitSlider(Slider slider, string key, Action<float> setVolume)
        {
            float value = PlayerPrefs.GetFloat(key, 1f);
            slider.value = value;
            setVolume.Invoke(value);
            slider.onValueChanged.AddListener(v =>
            {
                setVolume.Invoke(v);
                PlayerPrefs.SetFloat(key, v);
                PlayerPrefs.Save();
            });
        }
        private void OnFullscreenToggleChanged(bool isOn)
        {
            Screen.fullScreen = isOn;
            PlayerPrefs.SetInt("Fullscreen", isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
        private void GoToMainMenu()
        {
            _menuNavigation.ShowMainMenu();
        }
    }
}