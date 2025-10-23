using Core;
using Cysharp.Threading.Tasks;
using PrimeTween;
using System;
using UnityEngine;


namespace Systems
{
    public class SwapHandler
    {
        private GridSystem<GridObject<IGem>> _gridSystem;

        public SwapHandler(GridSystem<GridObject<IGem>> gridSystem)
        {
            _gridSystem = gridSystem;
        }

        //Clase swap
        public async UniTask SwapGems(Vector2Int gridPosA, Vector2Int gridPosB)
        {
            var gridObjectA = _gridSystem.GetValue(gridPosA.x, gridPosA.y); // Ver si hacer lo de velocidad aumaneta si no hay match y volvemos a su pocion original
            var gridObjectB = _gridSystem.GetValue(gridPosB.x, gridPosB.y);
            Tween.LocalPosition(gridObjectA.GetValue().Transform, _gridSystem.GetWorldPositionCenter(gridPosB.x, gridPosB.y), 0.5f, Ease.InQuad); //Moverlo a clase gem
            Tween.LocalPosition(gridObjectB.GetValue().Transform, _gridSystem.GetWorldPositionCenter(gridPosA.x, gridPosA.y), 0.5f, Ease.InQuad);

            _gridSystem.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
            _gridSystem.SetValue(gridPosB.x, gridPosB.y, gridObjectA);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }
}
