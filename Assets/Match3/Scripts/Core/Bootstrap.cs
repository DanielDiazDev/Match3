using Core;
using Level;
using Persistance;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Systems
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManagerPrefab;
        private void Awake()
        {
            if (!ServiceLocator.Instance.Has<GameManager>())
            {
                var gm = Instantiate(_gameManagerPrefab);
                DontDestroyOnLoad(gm.gameObject);
                ServiceLocator.Instance.Register(gm);
            }
            if (!ServiceLocator.Instance.Has<ISaveSystem>())
            {
                ISaveSystem saveSystem = new SaveSystem();
                ServiceLocator.Instance.Register(saveSystem);
            }
            if (!ServiceLocator.Instance.Has<ILevelProgress>())
            {
                ILevelProgress levelProgress = new LevelProgress();
                ServiceLocator.Instance.Register(levelProgress);
            }
            SceneManager.LoadScene("MainMenu");
        }
    }
}
