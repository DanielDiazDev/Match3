using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Systems.Score.Strategies
{
    [Serializable]
    public class PowerUpScoreStrategy : IScoreStrategy
    {
        [SerializeField] private int _basePowerUpPoints = 300;
        [SerializeField] private int _perGemBonus = 10;

        public int CalculateScore(int gemsDestroyed)
        {
            return _basePowerUpPoints + (gemsDestroyed * _perGemBonus);
        }
    }
}
