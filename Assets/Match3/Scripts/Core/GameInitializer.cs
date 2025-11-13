using Core;
using System.Sound;
using UnityEngine;


namespace Systems
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private Match3 _match;
        private void Start()
        {
            ServiceLocator.Instance.Get<SoundManager>().PlayMusic(SoundId.BackgroundMusic_2);
            var gameManager = ServiceLocator.Instance.Get<GameManager>();
            if (gameManager == null) return;
            _match.Init(gameManager.CurrentLevelSO);
        }
    }
}
