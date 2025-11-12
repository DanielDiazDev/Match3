using Level;
using UnityEngine;

namespace ScriptableObjects.Level.Objetives
{
    [CreateAssetMenu(fileName = "ObjectiveScoreSO", menuName = "Level/Objetive/ObjectiveScoreSO")]
    public class ObjectiveScoreSO : ObjectiveBaseSO
    {
        [SerializeField] private int targetScore;

        public override void Initialize(ObjectiveContext context)
        {
            context.CurrentScore = 0;
        }

        public override bool IsCompleted(ObjectiveContext context)
        {
            return context.CurrentScore >= targetScore;
        }
    }
}