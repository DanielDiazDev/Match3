using System;
using UnityEngine;

namespace Systems.Score.Strategies
{
    [Serializable]
    public class ComboScoreStrategy : IScoreStrategy
    {
        [SerializeField] private int _comboBaseBonus = 150;

        public int CalculateScore(int comboLevel)
        {
            return comboLevel * _comboBaseBonus;
        }
    }
}
