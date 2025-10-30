using Core;
using ScriptableObjects.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems;
using UI.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private LevelDatabaseSO _levelDatabaseSO;
        [SerializeField] private Transform _container;
        [SerializeField] private GameObject _levelButtonPrefab;
        private ILevelProgress _levelProgress;

        private void Start()
        {
            _levelProgress = ServiceLocator.Instance.Get<ILevelProgress>();
            _levelProgress.Initialize(_levelDatabaseSO.levels);
            GenerateLevelButton();
        }

        private void GenerateLevelButton()
        {
            foreach(Transform child in _container)
            {
                Destroy(child.gameObject);
            }
            foreach(var level in _levelDatabaseSO.levels)
            {
                var button = Instantiate(_levelButtonPrefab, _container);
                var levelButtonUI = button.GetComponent<LevelButtonUI>();
                var isUnlocked = _levelProgress.IsLevelUnlocked(level.levelID.ToString());
                levelButtonUI.Setup(level, isUnlocked, OnLevelSelected);
            }
        }

        private void OnLevelSelected(LevelSO level)
        {
            ServiceLocator.Instance.Get<GameManager>().SetCurrentLevel(level);
            SceneManager.LoadScene("Game");
        }
    }
}