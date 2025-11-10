using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using Systems.Score.Strategies;
using UnityEngine;

namespace Systems.Score
{
    public enum ScoreType
    {
        Match,
        PowerUp,
        Combo
    }
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; } //temporal
        public event Action<int> OnScoreChanged;
        public event Action< ScoreType, int> OnScoreEffectChanged;

        private int _currentScore = 0;
        public int CurrentScore => _currentScore;

        [Header("Score Strategies")]
        [SerializeField] private MatchScoreStrategy _matchStrategy;
        [SerializeField] private PowerUpScoreStrategy _powerUpStrategy;
        [SerializeField] private ComboScoreStrategy _comboStrategy;

        private Dictionary<ScoreType, IScoreStrategy> _scoreStrategies;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _scoreStrategies = new Dictionary<ScoreType, IScoreStrategy>
        {
            { ScoreType.Match, _matchStrategy },
            { ScoreType.PowerUp, _powerUpStrategy },
            { ScoreType.Combo, _comboStrategy }
        };
        }
        public void AddScore(ScoreType type, int value)
        {
            if (!_scoreStrategies.ContainsKey(type))
            {
                Debug.LogWarning($"No strategy found for score type: {type}");
                return;
            }
            var points = _scoreStrategies[type].CalculateScore(value);
            OnScoreEffectChanged?.Invoke(type, points);
            _currentScore += points;
            OnScoreChanged?.Invoke(_currentScore);
        }

        public void ResetScore()
        {
            _currentScore = 0;
            OnScoreChanged?.Invoke(_currentScore);
        }
    }
    public interface IScoreStrategy
    {
        int CalculateScore(int value);
    }
    
}