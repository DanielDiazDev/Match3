using Core;
using PrimeTween;
using ScriptableObjects;
using UnityEngine;


namespace Systems
{
    public class GemSpawner
    {
        private GemFactory _gemFactory;
        private GridSystem<GridObject<IGem>> _gridSystem;

        public GemSpawner(GemFactory gemFactory, GridSystem<GridObject<IGem>> gridSystem)
        {
            _gemFactory = gemFactory;
            _gridSystem = gridSystem;
        }

        //Clase spawner
        public IGem CreateGem(GemSO gemSO, int x, int y, Vector3 spawnPosition, bool animate = false) //Ver si no devuelve nada luego
        {
            var gem = _gemFactory.Create(gemSO, spawnPosition, Quaternion.identity);
            gem.Init(gemSO);

            // Verificar si ya existe un GridObject (por ejemplo, si hay un obstáculo)
            var gridObject = _gridSystem.GetValue(x, y);
            if (gridObject == null)
            {
                gridObject = new GridObject<IGem>(_gridSystem, x, y);
                _gridSystem.SetValue(x, y, gridObject);
            }
            
            gridObject.SetValue(gem);

            if (animate)
            {
                var target = _gridSystem.GetWorldPositionCenter(x, y);
                Tween.LocalPosition(gem.Transform, target, 0.3f, Ease.OutBounce);
            }

            return gem;
        }
    }
}
