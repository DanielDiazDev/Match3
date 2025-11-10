using Core;
using Cysharp.Threading.Tasks;
using ScriptableObjects;
using System.Threading.Tasks;
using UnityEngine;


namespace Systems
{
    public class PowerUpSystem : MonoBehaviour
    {
        [SerializeField] private BombSO _bombSO;
        [SerializeField] private LineHorizontalSO _lineHorizontalSO;
        [SerializeField] private LineVerticalSO _lineVerticalSO;
        [SerializeField] private GemClearSO _gemClearSO;
        private GridSystem<GridObject<IGem>> _gridSystem;
        private ExplodeSystem _explodeSystem;

        public void Init(GridSystem<GridObject<IGem>> gridSystem, ExplodeSystem explodeSystem)
        {
            _gridSystem = gridSystem;
            _explodeSystem = explodeSystem;
        }
        public bool HaveEnoughtToGeneratePowerUp(int count) => count > 3;
        public GemSO GetPowerUpSO(MatchPattern pattern)
        {
            return pattern switch
            {
                MatchPattern.LShape or MatchPattern.TShape or MatchPattern.Square => _bombSO,
                MatchPattern.LineVertical => _lineVerticalSO,
                MatchPattern.LineHorizontal => _lineHorizontalSO,
                MatchPattern.FiveInLine => _gemClearSO,
                _ => null,
            };
        }
        public void GeneratePowerUp(Gem oldGem, MatchData match, GemSO powerUpSO)
        {
            var pos = _gridSystem.GetWorldPositionCenter(match.Origin.x, match.Origin.y);
            var powerUp = Instantiate(oldGem, pos, Quaternion.identity, transform);
            powerUp.Init(powerUpSO);
            var newGridObject = _gridSystem.GetValue(match.Origin.x, match.Origin.y);
            newGridObject.SetValue(powerUp);
            _gridSystem.SetValue(match.Origin.x, match.Origin.y, newGridObject);
        }

        public async UniTask<int> ActivatePowerUp(IGem powerGem, Vector2Int gridPosA, Vector2Int gridPosB, int width, int height)
        {
            if (powerGem == null || powerGem.GetGem() == null)
                return 0;

            var gemSO = powerGem.GetGem();
            int destroyedCount = 0;

            if (gemSO is BombSO)
            {
                destroyedCount = await _explodeSystem.ExplodeArea(gridPosA, 1);
            }
            else if (gemSO is LineHorizontalSO)
            {
                destroyedCount = await _explodeSystem.ExplodeLine(gridPosA, true, width, height);
            }
            else if (gemSO is LineVerticalSO)
            {
                destroyedCount = await _explodeSystem.ExplodeLine(gridPosA, false, width, height);
            }
            else if (gemSO is GemClearSO)
            {
                var otherGem = _gridSystem.GetValue(gridPosB.x, gridPosB.y)?.GetValue() as Gem;
                if (otherGem != null)
                    destroyedCount = await _explodeSystem.ExplodeAllOfType(otherGem.GetGem(), width, height);
            }

            return destroyedCount;
        }
    }
}
