using Persistance;
using ScriptableObjects.Level;
using System.Linq;
using Systems;
using UnityEngine;

namespace Level
{
    public interface ILevelProgress
    {
        bool IsLevelUnlocked(string levelId);
        void UnlockLevel(string levelId);
        void Initialize(LevelSO[] levels);
        void SetStars(string levelID, int stars);
        int GetStars(string levelID);
    }
    public class LevelProgress : ILevelProgress
    {
        private GameData _gameData;
        public void Initialize(LevelSO[] levels)
        {
            var saveSystem = ServiceLocator.Instance.Get<ISaveSystem>();
            _gameData = saveSystem.Load();
            if (!_gameData.initialized)
            {
                if(levels.Length > 0)
                {
                    UnlockLevel(levels[0].levelID.ToString());
                }
                _gameData.initialized = true;
                saveSystem.Save(_gameData);
            }
        }
        public bool IsLevelUnlocked(string levelId)
        {
            var saveSystem = ServiceLocator.Instance.Get<ISaveSystem>();

            _gameData ??= saveSystem.Load();
            return _gameData.unlockedLevels.Contains(levelId);
        }

        public void UnlockLevel(string levelId)
        {
            var saveSystem = ServiceLocator.Instance.Get<ISaveSystem>();

            _gameData ??= saveSystem.Load();
            if (!_gameData.unlockedLevels.Contains(levelId))
            {
                _gameData.unlockedLevels.Add(levelId);
                saveSystem.Save(_gameData);
            }
        }
        public void SetStars(string levelID, int stars)
        {
            _gameData.levelStars.Add(levelID, stars);
        }

        public int GetStars(string levelID)
        {
            return _gameData.levelStars.TryGetValue(levelID, out var stars) ? stars : 0;
        }
    }
}