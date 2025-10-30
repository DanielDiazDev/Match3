using Core;
using UnityEngine;


namespace Systems
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private Match3 _match;
        private void Start()
        {
            var gameManager = ServiceLocator.Instance.Get<GameManager>();
            _match.Init(gameManager.CurrentLevelSO);
        }
    }
}
