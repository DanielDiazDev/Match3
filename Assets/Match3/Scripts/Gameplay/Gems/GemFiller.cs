using Core;
using Cysharp.Threading.Tasks;
using ScriptableObjects;
using System;
using UnityEngine;


namespace Systems
{
    public class GemFiller
    {
        private GridSystem<GridObject<IGem>> _gridSystem;
        private int _width;
        private int _height;
        private GemSpawner _gemSpawner;
        public GemFiller(GridSystem<GridObject<IGem>> gridSystem, int width, int height, GemSpawner gemSpawner)
        {
            _gridSystem = gridSystem;
            _width = width;
            _height = height;
            _gemSpawner = gemSpawner;
        }

        public async UniTask FillEmptySpots(GemSO[] gemTypes)
        {
            float maxDuration = 0.3f;

            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    var gridObject = _gridSystem.GetValue(x, y);
                    if (gridObject == null || gridObject.GetValue() == null)
                    {
                        _gemSpawner.CreateGem(gemTypes[UnityEngine.Random.Range(0, gemTypes.Length)], x, y, _gridSystem.GetWorldPositionCenter(x, _height + 1), true);
                        //audioManager.PlayPop();
                    }
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(maxDuration));
        }
    }
}
