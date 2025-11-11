using ScriptableObjects;
using UnityEngine;

namespace Core
{
    public interface IObstacle
    {
        void Init(ObstacleSO obstacleSO);
        Transform Transform { get; }
        ObstacleSO GetObstacle();
        int GetHitsRemaining();
        void Hit();
        bool IsDestroyed();
    }

    public class Obstacle : MonoBehaviour, IObstacle
    {
        [field: SerializeField] public ObstacleSO ObstacleSO { get; private set; }
        private int _hitsRemaining;

        public Transform Transform => transform;

        public void Init(ObstacleSO obstacleSO)
        {
            ObstacleSO = obstacleSO;
            _hitsRemaining = obstacleSO.HitsRequired;
            GetComponent<SpriteRenderer>().sprite = obstacleSO.Icon;
        }

        public ObstacleSO GetObstacle() => ObstacleSO;

        public int GetHitsRemaining() => _hitsRemaining;

        public void Hit()
        {
            _hitsRemaining--;
            if (_hitsRemaining <= 0)
            {
                _hitsRemaining = 0;
            }
        }

        public bool IsDestroyed() => _hitsRemaining <= 0;
    }
}

