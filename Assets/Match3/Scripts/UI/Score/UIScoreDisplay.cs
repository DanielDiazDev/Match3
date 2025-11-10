using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems.Score;
using TMPro;
using UnityEngine;

namespace UI.Score
{
    public class UIScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _scoreText;
        [SerializeField] private ScoreManager _scoreManager;

        private void OnEnable()
        {
           // ScoreManager.Instance.OnScoreChanged += UpdateScore;
            _scoreManager.OnScoreChanged += UpdateScore;
            _scoreManager.OnScoreEffectChanged += ShowEffectScore;
        }

      
        private void OnDisable()
        {
            // ScoreManager.Instance.OnScoreChanged -= UpdateScore;
            _scoreManager.OnScoreChanged -= UpdateScore;
            _scoreManager.OnScoreEffectChanged -= ShowEffectScore;

        }

        private void UpdateScore(int newScore)
        {
            // _scoreText.text = $"Score: {newScore}";
            Debug.Log($"Score: {newScore}");
        }
        private void ShowEffectScore(ScoreType type, int score)
        {
            Debug.Log($"Efecto: {type}, {score}");

        }

    }
}
