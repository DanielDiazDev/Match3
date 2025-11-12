using Level;
using UnityEngine;

namespace ScriptableObjects.Level.Objetives
{
    [CreateAssetMenu(fileName = "ObjectiveRemoveObstaclesSO", menuName = "Level/Objetive/ObjectiveRemoveObstaclesSO")]
    public class ObjectiveRemoveObstaclesSO : ObjectiveBaseSO
    {
        [SerializeField] private int requiredCount;

        public override void Initialize(ObjectiveContext context)
        {
            context.ObstaclesDestroyed = 0;
        }

        public override bool IsCompleted(ObjectiveContext context)
        {
            return context.ObstaclesDestroyed >= requiredCount;
        }
    }
}