using Core;
using Cysharp.Threading.Tasks;
using PrimeTween;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Systems
{
    public class ExplodeSystem
    {
        private GridSystem<GridObject<IGem>> _gridSystem;

        public ExplodeSystem(GridSystem<GridObject<IGem>> gridSystem)
        {
            _gridSystem = gridSystem;
        }

        public async UniTask ExplodeGems(List<Vector2Int> matches)
        {
            if (matches == null || matches.Count == 0)
                return;

            float maxDuration = 0.25f; // Duración máxima del tween

            foreach (var match in matches)
            {
                var gridObj = _gridSystem.GetValue(match.x, match.y);
                if (gridObj == null) continue;

                var gem = gridObj.GetValue();
                _gridSystem.SetValue(match.x, match.y, null);
                // ExplodeVFX()
                Tween.PunchScale(gem.Transform, Vector3.one * 0.2f, 0.15f, frequency: 2);
                Tween.Scale(gem.Transform, Vector3.zero, 0.2f, Ease.InBack);

                GameObject.Destroy((gem as Gem).gameObject, maxDuration); //Ponerlo en la clase gem
            }

            // Esperar la duración más larga de las animaciones
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
