using UnityEngine;
using Level;

namespace ScriptableObjects.Level.Objetives
{
    [CreateAssetMenu(fileName = "ObjectiveScoreSO", menuName = "Level/Objective/Score")]
    public class ObjectiveScoreSO : ObjectiveBaseSO
    {
        public override void Initialize(ObjectiveContext context)
        {
            context.CurrentScore = 0;
        }

        public override int GetCurrentValue(ObjectiveContext context)
        {
            return context.CurrentScore;
        }
        public override string GetResolvedDescription(ObjectiveContext context)
        {
            string desc = base.GetResolvedDescription(context);
            desc = desc.Replace("{remaining}", context.CurrentScore.ToString()); 
            return desc;
        }
    }
}
