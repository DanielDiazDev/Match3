using Level;
using UnityEngine;

namespace ScriptableObjects.Level.Objetives
{
    [CreateAssetMenu(fileName = "ObjectiveRemoveGemSO", menuName = "Level/Objetive/ObjectiveRemoveGemSO")]
    public class ObjectiveRemoveGemSO : ObjectiveBaseSO
    {
        [SerializeField] private GemSO targetGem;
        [SerializeField] private int requiredAmount;

        public override void Initialize(ObjectiveContext context)
        {
            context.GemsDestroyed[targetGem.name] = 0;
        }

        public override bool IsCompleted(ObjectiveContext context)
        {
            return context.GemsDestroyed.TryGetValue(targetGem.name, out var count) && count >= requiredAmount;
        }
    }
}