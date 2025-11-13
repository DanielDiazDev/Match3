using UnityEngine;
using Level;

namespace ScriptableObjects.Level.Objetives
{
    [CreateAssetMenu(fileName = "ObjectiveRemoveObstaclesSO", menuName = "Level/Objective/Remove Obstacles")]
    public class ObjectiveRemoveObstaclesSO : ObjectiveBaseSO
    {
        public override void Initialize(ObjectiveContext context)
        {
            context.ObstaclesDestroyed = 0;
        }

        public override int GetCurrentValue(ObjectiveContext context)
        {
            return context.ObstaclesDestroyed;
        }
        public override string GetResolvedDescription(ObjectiveContext context)
        {
            string desc = base.GetResolvedDescription(context);
            desc = desc.Replace("{remaining}", context.ObstaclesDestroyed.ToString());
            return desc;
        }
    }
}
