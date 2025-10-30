using Core;
using Cysharp.Threading.Tasks;
using PrimeTween;
using ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Systems
{
    public class ExplodeSystem : MonoBehaviour
    {
        private GridSystem<GridObject<IGem>> _gridSystem;
        [SerializeField] private PowerUpSystem _powerUpSystem;
        public void Init(GridSystem<GridObject<IGem>> gridSystem)
        {
            _gridSystem = gridSystem;
        }

        public async UniTask ExplodeGems(List<MatchData> matches)
        {
            if (matches == null || matches.Count == 0)
                return;

            float maxDuration = 0.25f;

            foreach (var match in matches)
            {
                GemSO powerUpSO = null;
                if (_powerUpSystem.HaveEnoughtToGeneratePowerUp(match.Positions.Count))
                {
                    powerUpSO = _powerUpSystem.GetPowerUpSO(match.Pattern);
                }

                if (powerUpSO != null)
                {
                    var oldGridObject = _gridSystem.GetValue(match.Origin.x, match.Origin.y);
                    if (oldGridObject != null && oldGridObject.GetValue() is Gem oldGem)
                    {
                        GameObject.Destroy(oldGem.gameObject, maxDuration);
                        _powerUpSystem.GeneratePowerUp(oldGem, match, powerUpSO);
                       
                    }
                }

                foreach (var pos in match.Positions)
                {
                    if (powerUpSO != null && pos == match.Origin)
                        continue;

                    var gridObj = _gridSystem.GetValue(pos.x, pos.y);
                    if (gridObj == null) continue;

                    var gem = gridObj.GetValue();
                    if (gem == null) continue;

                    _gridSystem.SetValue(pos.x, pos.y, null);

                    _= Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                    _ = Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);
                    GameObject.Destroy((gem as Gem).gameObject, maxDuration);
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));
        }

        public async UniTask ExplodeArea(Vector2Int center, int radius)
        {
            float maxDuration = 0.25f;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2Int pos = new(center.x + dx, center.y + dy);
                    var gridObj = _gridSystem.GetValue(pos.x, pos.y);
                    if (gridObj?.GetValue() == null) continue;

                    var gem = gridObj.GetValue();
                    _gridSystem.SetValue(pos.x, pos.y, null);

                    _ = Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                    _ = Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);
                    GameObject.Destroy((gem as Gem).gameObject, maxDuration);
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));
        }


        public async UniTask ExplodeLine(Vector2Int origin, bool horizontal, int width, int height)
        {
            float maxDuration = 0.25f;

            int length = horizontal ? width : height;

            for (int i = 0; i < length; i++)
            {
                int x = horizontal ? i : origin.x;
                int y = horizontal ? origin.y : i;

                var gridObj = _gridSystem.GetValue(x, y);
                if (gridObj?.GetValue() == null) continue;

                var gem = gridObj.GetValue();
                _gridSystem.SetValue(x, y, null);

                _ = Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                _ = Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);
                GameObject.Destroy((gem as Gem).gameObject, maxDuration);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));
        }


        public async Task ExplodeAllOfType(GemSO gemSO, int width, int height)
        {
            float maxDuration = 0.25f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var gridObj = _gridSystem.GetValue(x, y);
                    if (gridObj?.GetValue() == null) continue;

                    var gem = gridObj.GetValue();
                    if (gem.GetGem() != gemSO) continue;

                    _gridSystem.SetValue(x, y, null);

                    _ = Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                    _ = Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);
                    GameObject.Destroy((gem as Gem).gameObject, maxDuration);
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));
        }





        //void ExplodeVFX(Vector2Int match)
        //{
        //    // TODO: Pool
        //    var fx = Instantiate(explosion, transform);
        //    fx.transform.position = _gridSystem.GetWorldPositionCenter(match.x, match.y);
        //    Destroy(fx, 5f);
        //}
    }
}
