using Core;
using Level;
using ScriptableObjects.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems;
using Systems.Score;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIEndGame : MonoBehaviour
    {
        [SerializeField] private Image[] _stars;
        [SerializeField] private Sprite _fullStar;
        [SerializeField] private TextMeshProUGUI _scoreFinalText;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private Button _btnNextLevel;
        [SerializeField] private Button _btnGoToMainMenu;
        [SerializeField] private Button _btnReset;
        [SerializeField] private LevelDatabaseSO _levelDatabaseSO;
        private ObjectiveSystem _objectiveSystem;
        private GameManager _gameManager;
        private void OnEnable()
        {
            _objectiveSystem = ServiceLocator.Instance.Get<ObjectiveSystem>();
            _objectiveSystem.OnStarsEarned += UpdateStars;
        }
        private void Start()
        {
            UpdateScore();
            
            _btnNextLevel.onClick.AddListener(GoToNextLevel);
            _btnGoToMainMenu.onClick.AddListener(GoToMainMenu);
            _btnReset.onClick.AddListener(ResetLevel);
            var levels = _levelDatabaseSO.levels;
            _gameManager = ServiceLocator.Instance.Get<GameManager>();
            var currentLevel = _gameManager.CurrentLevelSO;
            if (_objectiveSystem.IsLevelComplete())
            {
                _resultText.text = "Victory";
                ServiceLocator.Instance.Get<ILevelProgress>().SetStars(currentLevel.levelID.ToString(), _objectiveSystem.GetStarCount());
            }
            else
            {
                _resultText.text = "Failure";
            }
            var currentIndex = Array.IndexOf(levels, currentLevel);
            _btnNextLevel.interactable = currentIndex >= 0 && currentIndex < levels.Length - 1;
        }

        private void ResetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private void GoToNextLevel()
        {
            var levels = _levelDatabaseSO.levels;
            var currentLevel = _gameManager.CurrentLevelSO;

            var currentIndex = Array.IndexOf(levels, currentLevel);
            var next = levels[currentIndex + 1];
            _gameManager.SetCurrentLevel(next);
            SceneManager.LoadScene("Game");
        }

        private void OnDestroy()
        {
            _objectiveSystem.OnStarsEarned -= UpdateStars;
        }
        private void UpdateScore()
        {
            _scoreFinalText.text = ServiceLocator.Instance.Get<ScoreManager>().GetScore().ToString();
        }


        public void UpdateStars(int starsEarned)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                if (i < starsEarned)
                {
                    _stars[i].sprite = _fullStar;
                }
            }
        }
        public void ShowEnd()
        {
            gameObject.SetActive(true);
        }
    }
}