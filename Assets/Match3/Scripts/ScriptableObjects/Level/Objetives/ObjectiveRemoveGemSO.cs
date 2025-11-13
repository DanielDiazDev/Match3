using UnityEngine;
using Level;

namespace ScriptableObjects.Level.Objetives
{
    [CreateAssetMenu(fileName = "ObjectiveRemoveGemSO", menuName = "Level/Objective/Remove Gem")]
    public class ObjectiveRemoveGemSO : ObjectiveBaseSO
    {
        [SerializeField] private GemSO targetGem;

        public override void Initialize(ObjectiveContext context)
        {
            context.GemsDestroyed[targetGem.name] = 0;
        }

        public override int GetCurrentValue(ObjectiveContext context)
        {
            if (context.GemsDestroyed.TryGetValue(targetGem.name, out var count))
                return count;
            return 0;
        }
        public override string GetResolvedDescription(ObjectiveContext context)
        {
            string desc = base.GetResolvedDescription(context);
            int remaining = GetRemaining(context);
            desc = desc
                .Replace("{gem}", targetGem.name)
                .Replace("{remaining}", remaining.ToString());

            return desc;
        }
    }
}
