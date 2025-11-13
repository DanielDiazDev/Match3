using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

namespace System.Sound
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Clips")]
        [SerializeField] private SerializedDictionary<SoundId, AudioClip> _clips;
        [Header("Volumes")]
        [Range(0f, 1f)] private float _masterVolume = 1f;
        [Range(0f, 1f)] private float _musicVolume = 1f;
        [Range(0f, 1f)] private float _sfxVolume = 1f;
        private void Awake()
        {
            _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
        private void Update()
        {
            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            _musicSource.volume = _musicVolume * _masterVolume;
            _sfxSource.volume = _sfxSource.volume * _masterVolume;
        }

        public void PlayMusic(SoundId soundId, bool loop = true)
        {
            if (!_clips.TryGetValue(soundId, out var clip)) return;
            if (_musicSource.clip == clip) return;
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }
        public void StopMusic() => _musicSource.Stop();
        public void PlaySFX(SoundId soundId)
        {
            if (!_clips.TryGetValue(soundId, out var clip)) return;
            _sfxSource.PlayOneShot(clip, _sfxVolume * _masterVolume);
        }

        public void SetMasterVolume(float volume) => _masterVolume = Mathf.Clamp01(volume);
        public void SetMusicVolume(float volume) => _musicVolume = Mathf.Clamp01(volume);
        public void SetSFXVolume(float volume) => _sfxVolume = Mathf.Clamp01(volume);
    }
    public enum SoundId
    {
        BackgroundMusic_1,
        BackgroundMusic_2,
        BackgroundMusic_3,
        Swap,
        Match,
        Gameover,
        Victory
    }
}
