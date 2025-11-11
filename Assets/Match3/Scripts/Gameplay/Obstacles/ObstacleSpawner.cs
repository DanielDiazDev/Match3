using Core;
using ScriptableObjects;
using UnityEngine;

namespace Systems
{
    public class ObstacleSpawner
    {
        private ObstacleFactory _obstacleFactory;
        private GridSystem<GridObject<IGem>> _gridSystem;

        public ObstacleSpawner(ObstacleFactory obstacleFactory, GridSystem<GridObject<IGem>> gridSystem)
        {
            _obstacleFactory = obstacleFactory;
            _gridSystem = gridSystem;
        }

        public IObstacle CreateObstacle(ObstacleSO obstacleSO, int x, int y, Vector3 spawnPosition)
        {
            var obstacle = _obstacleFactory.Create(obstacleSO, spawnPosition, Quaternion.identity);
            obstacle.Init(obstacleSO);

            var gridObject = _gridSystem.GetValue(x, y);
            if (gridObject == null)
            {
                gridObject = new GridObject<IGem>(_gridSystem, x, y);
                _gridSystem.SetValue(x, y, gridObject);
            }

            gridObject.SetObstacle(obstacle);

            // Ajustar la posición Z del obstáculo para que esté detrás de la gema
            var obstacleTransform = obstacle.Transform;
            var pos = obstacleTransform.position;
            pos.z = 0.1f; // Obstáculo detrás de la gema
            obstacleTransform.position = pos;

            return obstacle;
        }
    }
}

