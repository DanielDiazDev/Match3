using Core;
using Cysharp.Threading.Tasks;
using PrimeTween;
using System;


namespace Systems
{
    public class GravityManager
    {
        private GridSystem<GridObject<IGem>> _gridSystem;
        private int _width;
        private int _height;

        public GravityManager(GridSystem<GridObject<IGem>> gridSystem, int width, int height)
        {
            _gridSystem = gridSystem;
            _width = width;
            _height = height;
        }

        public async UniTask MakeGemsFall()
        {
            float maxDuration = 0.25f;
            bool moved = false;

            for (var x = 0; x < _width; x++)
            {
                int emptyY = -1;
                for (var y = 0; y < _height; y++)
                {
                    if (_gridSystem.GetValue(x, y) == null)
                    {
                        if (emptyY == -1) emptyY = y;
                    }
                    else if (emptyY != -1)
                    {
                        moved = true;
                        var gridObject = _gridSystem.GetValue(x, y);
                        var gem = gridObject.GetValue();
                        _gridSystem.SetValue(x, emptyY, gridObject);
                        _gridSystem.SetValue(x, y, null);

                        var targetPos = _gridSystem.GetWorldPositionCenter(x, emptyY);
                        _ = Tween.LocalPosition(gem.Transform, targetPos, maxDuration, Ease.InQuad);

                        emptyY++;
                    }
                }
            }

            if (moved)
                await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));

        }
    }
}
