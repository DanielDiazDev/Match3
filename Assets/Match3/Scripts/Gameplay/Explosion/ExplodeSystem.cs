using Core;
using Cysharp.Threading.Tasks;
using PrimeTween;
using ScriptableObjects;
using System;
using System.Collections.Generic;
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

        public async UniTask<int> ExplodeGems(List<MatchData> matches)
        {
            if (matches == null || matches.Count == 0)
                return 0;

            float maxDuration = 0.25f;
            int destroyedCount = 0;

            foreach (var match in matches)
            {
                GemSO powerUpSO = null;
                if (_powerUpSystem.HaveEnoughtToGeneratePowerUp(match.Positions.Count))
                {
                    powerUpSO = _powerUpSystem.GetPowerUpSO(match.Pattern);
                }

                // Generar PowerUp si aplica
                if (powerUpSO != null)
                {
                    var oldGridObject = _gridSystem.GetValue(match.Origin.x, match.Origin.y);
                    if (oldGridObject != null && oldGridObject.GetValue() is Gem oldGem)
                    {
                        GameObject.Destroy(oldGem.gameObject, maxDuration);
                        _powerUpSystem.GeneratePowerUp(oldGem, match, powerUpSO);
                    }
                }

                // Destruir gemas del match
                foreach (var pos in match.Positions)
                {
                    var gridObj = _gridSystem.GetValue(pos.x, pos.y);
                    if (gridObj == null) continue;

                    Debug.Log($"Explode check at {pos}: HasObstacle={gridObj.HasObstacle()}, HasGem={gridObj.GetValue() != null}");

                    // 1) SIEMPRE golpear obstáculo si existe (independiente de si es origen)
                    if (gridObj.HasObstacle())
                    {
                        var obstacle = gridObj.GetObstacle();
                        obstacle.Hit();
                        if (obstacle.IsDestroyed())
                        {
                            _ = Tween.PunchScale(obstacle.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                            _ = Tween.Scale(obstacle.Transform, Vector3.zero, 0.2f, Ease.InBack);
                            GameObject.Destroy((obstacle as Obstacle).gameObject, maxDuration);
                            gridObj.RemoveObstacle();
                        }
                    }

                    // 2) Si corresponde generar powerUp en esta posición (es el origen), 
                    //    no destruyas la gema aquí — eso lo maneja la generación del powerup.
                    if (powerUpSO != null && pos == match.Origin)
                    {
                        // Asegurarnos de que el powerUp se genere correctamente:
                        var oldGridObject = gridObj;
                        if (oldGridObject != null && oldGridObject.GetValue() is Gem oldGem)
                        {
                            // si ya habías planeado destruir el oldGem y generar powerup, mantenlo
                            GameObject.Destroy(oldGem.gameObject, maxDuration);
                            _powerUpSystem.GeneratePowerUp(oldGem, match, powerUpSO);
                        }

                        // no seguir con la destrucción de gema normal
                        continue;
                    }

                    // 3) Para todas las demás posiciones: destruir la gema si existe
                    var gem = gridObj.GetValue();
                    if (gem == null) continue;

                    _gridSystem.SetValue(pos.x, pos.y, null);

                    _ = Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                    _ = Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);
                    GameObject.Destroy((gem as Gem).gameObject, maxDuration);

                    destroyedCount++;
                }
            }

                await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));
            return destroyedCount;
        }

        public async UniTask<int> ExplodeArea(Vector2Int center, int radius)
        {
            float maxDuration = 0.25f;
            int destroyedCount = 0;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Vector2Int pos = new(center.x + dx, center.y + dy);
                    var gridObj = _gridSystem.GetValue(pos.x, pos.y);
                    if (gridObj == null) continue;

                    // Destruir obstáculo si existe
                    if (gridObj.HasObstacle())
                    {
                        var obstacle = gridObj.GetObstacle();
                        obstacle.Hit();
                        if (obstacle.IsDestroyed())
                        {
                            _ = Tween.PunchScale(obstacle.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                            _ = Tween.Scale(obstacle.Transform, Vector3.zero, 0.2f, Ease.InBack);
                            GameObject.Destroy((obstacle as Obstacle).gameObject, maxDuration);
                            gridObj.RemoveObstacle();
                        }
                    }

                    if (gridObj.GetValue() == null) continue;

                    var gem = gridObj.GetValue();
                    _gridSystem.SetValue(pos.x, pos.y, null);

                    _ = Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                    _ = Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);
                    GameObject.Destroy((gem as Gem).gameObject, maxDuration);

                    destroyedCount++;
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));
            return destroyedCount;
        }

        public async UniTask<int> ExplodeLine(Vector2Int origin, bool horizontal, int width, int height)
        {
            float maxDuration = 0.25f;
            int destroyedCount = 0;

            int length = horizontal ? width : height;

            for (int i = 0; i < length; i++)
            {
                int x = horizontal ? i : origin.x;
                int y = horizontal ? origin.y : i;

                var gridObj = _gridSystem.GetValue(x, y);
                if (gridObj == null) continue;

                // Destruir obstáculo si existe
                if (gridObj.HasObstacle())
                {
                    var obstacle = gridObj.GetObstacle();
                    obstacle.Hit();
                    if (obstacle.IsDestroyed())
                    {
                        _ = Tween.PunchScale(obstacle.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                        _ = Tween.Scale(obstacle.Transform, Vector3.zero, 0.2f, Ease.InBack);
                        GameObject.Destroy((obstacle as Obstacle).gameObject, maxDuration);
                        gridObj.RemoveObstacle();
                    }
                }

                if (gridObj.GetValue() == null) continue;

                var gem = gridObj.GetValue();
                _gridSystem.SetValue(x, y, null);

                _ = Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                _ = Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);
                GameObject.Destroy((gem as Gem).gameObject, maxDuration);

                destroyedCount++;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));
            return destroyedCount;
        }

        public async UniTask<int> ExplodeAllOfType(GemSO gemSO, int width, int height)
        {
            float maxDuration = 0.25f;
            int destroyedCount = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var gridObj = _gridSystem.GetValue(x, y);
                    if (gridObj == null) continue;

                    // Destruir obstáculo si existe
                    if (gridObj.HasObstacle())
                    {
                        var obstacle = gridObj.GetObstacle();
                        obstacle.Hit();
                        if (obstacle.IsDestroyed())
                        {
                            _ = Tween.PunchScale(obstacle.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                            _ = Tween.Scale(obstacle.Transform, Vector3.zero, 0.2f, Ease.InBack);
                            GameObject.Destroy((obstacle as Obstacle).gameObject, maxDuration);
                            gridObj.RemoveObstacle();
                        }
                    }

                    if (gridObj.GetValue() == null) continue;

                    var gem = gridObj.GetValue();
                    if (gem.GetGem() != gemSO) continue;

                    _gridSystem.SetValue(x, y, null);

                    _ = Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                    _ = Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);
                    GameObject.Destroy((gem as Gem).gameObject, maxDuration);

                    destroyedCount++;
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));
            return destroyedCount;
        }
    }
}






        //void ExplodeVFX(Vector2Int match)
        //{
        //    // TODO: Pool
        //    var fx = Instantiate(explosion, transform);
        //    fx.transform.position = _gridSystem.GetWorldPositionCenter(match.x, match.y);
        //    Destroy(fx, 5f);
        //}
