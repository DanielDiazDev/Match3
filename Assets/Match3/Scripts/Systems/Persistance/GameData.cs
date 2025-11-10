using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Persistance
{
    [Serializable]
    public class GameData
    {
        public bool initialized;
        public List<string> unlockedLevels = new();
    }
    public interface ISaveSystem
    {
        void Save(GameData gameData);
        GameData Load();
        void Delete();
    }
    public class SaveSystem : ISaveSystem
    {
        private readonly string _savePath = Path.Combine(Application.persistentDataPath, "save.json");
        public void Save(GameData gameData)
        {
            try
            {
                string json = JsonUtility.ToJson(gameData, true);
                File.WriteAllText(_savePath, json);
#if UNITY_EDITOR
                Debug.Log($"[SaveSystem] Game saved to: {_savePath}");
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Error saving file: {e}");
            }
        }
        public GameData Load()
        {
            if (!File.Exists(_savePath))
            {
#if UNITY_EDITOR
                Debug.Log("[SaveSystem] Save file not found, creating new one.");
#endif
                return new GameData();
            }
            try
            {
                var json = File.ReadAllText(_savePath);
                return JsonUtility.FromJson<GameData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Error loading file: {e}");
                return new GameData();
            }
        }
        public void Delete()
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
#if UNITY_EDITOR
                Debug.Log("[SaveSystem] Save file deleted.");
#endif
            }
        }
    }
}
