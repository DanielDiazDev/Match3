using AYellowpaper.SerializedCollections;
using ScriptableObjects;
using ScriptableObjects.Level.Objetives;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Level
{
    [CreateAssetMenu(fileName = "LevelSO", menuName = "Level/LevelSO")]
    public class LevelSO : ScriptableObject
    {
        public int levelID;
        public List<ObjectiveBaseSO> objetives;
        public int moveLimit;
        public int width;
        public int height;
        public SerializedDictionary<Vector2Int, GemSO> initialGems;
        public SerializedDictionary<Vector2Int, ObstacleSO> initialObstacles;
    }
}