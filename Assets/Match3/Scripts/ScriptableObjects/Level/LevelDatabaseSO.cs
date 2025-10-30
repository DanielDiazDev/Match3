using UnityEngine;

namespace ScriptableObjects.Level
{
    [CreateAssetMenu(fileName = "LevelDatabaseSO", menuName = "Level/LevelDatabaseSO")]
    public class LevelDatabaseSO : ScriptableObject
    {
        public LevelSO[] levels;
    }
}