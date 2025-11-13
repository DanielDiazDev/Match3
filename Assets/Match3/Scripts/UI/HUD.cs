using Core;
using System;
using Systems;
using Systems.Score;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _currentScoreText;
        [SerializeField] private TextMeshProUGUI _moveLeftText;
        private GameManager _gameManager;
        [SerializeField] private ScoreManager _scoreManager;
       // [SerializeField] private Image[] _icons;
        [SerializeField] private ObjectiveIconUI[] _icons;
        [SerializeField] private TooltipUI _tooltip;
        private void OnEnable()
        {
            _gameManager = ServiceLocator.Instance.Get<GameManager>();
            _scoreManager.OnScoreChanged += UpdateScore;
            _gameManager.OnMoveLeftChanged += UpdateMoveLeft;
        }

        private void OnDestroy()
        {
            _scoreManager.OnScoreChanged -= UpdateScore;
            _gameManager.OnMoveLeftChanged -= UpdateMoveLeft;
        }

        private void UpdateMoveLeft(int movesLeft)
        {
            _moveLeftText.text = movesLeft.ToString();
        }
        private void Start()
        {
            DisplayObjectives();
            UpdateMoveLeft(_gameManager.CurrentLevelSO.moveLimit);
            _levelText.text = _gameManager.CurrentLevelSO.levelID.ToString();
        }
        private void UpdateScore(int score)
        {
            _currentScoreText.text = score.ToString();
        } 
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        private void DisplayObjectives()
        {
            var objectives = _gameManager.CurrentLevelSO.objetives;
            for (int i = 0; i < _gameManager.CurrentLevelSO.objetives.Count; i++)
            {
                _icons[i].Initialize(objectives[i], _tooltip);
            }
        }
    }
}