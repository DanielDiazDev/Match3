using Core;
using ScriptableObjects;
using UnityEngine;

namespace Systems
{
    public class ObstacleFactory
    {
        private readonly Transform _parent;

        public ObstacleFactory(Transform parent)
        {
            _parent = parent;
        }

        public IObstacle Create(ObstacleSO obstacleSO, Vector2 position, Quaternion rotation)
        {
            return UnityEngine.Object.Instantiate(obstacleSO.Prefab, position, rotation, _parent).GetComponent<IObstacle>();
        }
    }
}

