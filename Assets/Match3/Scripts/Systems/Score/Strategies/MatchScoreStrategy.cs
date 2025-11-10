using System;
using UnityEngine;

namespace Systems.Score.Strategies
{
    [Serializable]
    public class MatchScoreStrategy : IScoreStrategy
    {
        [SerializeField] private int _basePoints = 100;
        public int CalculateScore(int matchSize)
        {
            var multiplier = 1f;
            if (matchSize == 4) multiplier = 1.5f;
            else if (matchSize >= 5) multiplier = 2f;
            return Mathf.RoundToInt(_basePoints * multiplier);
        }
    }
}
