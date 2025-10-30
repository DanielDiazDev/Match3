using ScriptableObjects.Level;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public LevelSO CurrentLevelSO {  get; private set; }
        public void SetCurrentLevel(LevelSO levelSO)
        {
            CurrentLevelSO = levelSO;
        }
    }

}