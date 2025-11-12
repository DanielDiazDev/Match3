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

public class UIEndGame : MonoBehaviour
{
    [SerializeField] private Image[] _stars;
    [SerializeField] private Sprite _fullStar;
    [SerializeField] private TextMeshProUGUI _scoreFinalText;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Button _btnNextLevel;
    [SerializeField] private Button _btnLevelSelector;
    [SerializeField] private Button _btnReset;
    [SerializeField] private LevelDatabaseSO _levelDatabaseSO;
    private ObjectiveSystem _objectiveSystem;
    private void OnEnable()
    {
        _objectiveSystem = ServiceLocator.Instance.Get<ObjectiveSystem>();
        _objectiveSystem.OnStarsEarned += UpdateStars;
    }
    private void Start()
    {
        UpdateScore();
        _resultText.text = _objectiveSystem.IsLevelComplete() ? "Victory" : "Failure";
        _btnNextLevel.onClick.AddListener(GoToNextLevel);
        _btnLevelSelector.onClick.AddListener(GoToLevelSelector);
        _btnReset.onClick.AddListener(ResetLevel);
    }

    private void ResetLevel()
    {
        throw new NotImplementedException();
    }

    private void GoToLevelSelector()
    {
        throw new NotImplementedException();
    }

    private void GoToNextLevel()
    {
        var gameManager = ServiceLocator.Instance.Get<GameManager>();
        var levels = _levelDatabaseSO.levels;
        var currentLevel = gameManager.CurrentLevelSO;

        var currentIndex = Array.IndexOf(levels, currentLevel);
        if (currentIndex >= 0 && currentIndex < levels.Length - 1)
        {
            var siguiente = levels[currentIndex + 1];
            gameManager.SetCurrentLevel(siguiente);
            SceneManager.LoadScene("Game");
        }
        else
        {
            _btnNextLevel.interactable = false;
        }
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
        for(int i = 0; i < _stars.Length; i++)
        {
            if(i < starsEarned)
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
