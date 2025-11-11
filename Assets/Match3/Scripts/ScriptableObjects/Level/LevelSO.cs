using AYellowpaper.SerializedCollections;
using ScriptableObjects;
using UnityEngine;

namespace ScriptableObjects.Level
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "Level/LevelSO")]
    public class LevelSO : ScriptableObject
    {
        public int levelID;
        public SerializedDictionary<int, int> scores;
        public SerializedDictionary<GemSO, int> objetivesGems;
        public int moveLimit;
        public int width;
        public int height;
        public SerializedDictionary<Vector2Int, GemSO> initialGems;
        public SerializedDictionary<Vector2Int, ObstacleSO> initialObstacles;
        public AudioClip themeMusic;
        public Sprite backgroundSprite;
       // public bool isUnlocked; Ver si este o usar prefs
        //  public LevelTheme theme;
        // public Difficulty difficulty;
    }
}