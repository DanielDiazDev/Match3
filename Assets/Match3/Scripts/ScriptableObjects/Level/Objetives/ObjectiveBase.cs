using UnityEngine;
using Level;

namespace ScriptableObjects.Level.Objetives
{
    public abstract class ObjectiveBaseSO : ScriptableObject
    {
        [Header("Base Data")]
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _name;
        [SerializeField] private string _description;

        [Header("Objective Settings")]
        [SerializeField] protected int _targetValue = 1;

        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int TargetValue => _targetValue;


        public abstract void Initialize(ObjectiveContext context);
        public abstract int GetCurrentValue(ObjectiveContext context);

        public virtual bool IsCompleted(ObjectiveContext context)
        {
            return GetCurrentValue(context) >= _targetValue;
        }
        public int GetRemaining(ObjectiveContext context)
        {
            return Mathf.Max(0, _targetValue - GetCurrentValue(context));
        }

        public float GetProgress(ObjectiveContext context)
        {
            return Mathf.Clamp01((float)GetCurrentValue(context) / _targetValue);
        }
        public virtual string GetResolvedDescription(ObjectiveContext context)
        {
            if (context == null)
                return _description;

            var desc = _description;
            desc = desc.Replace("{remaining}", GetRemaining(context).ToString());
            return desc;
        }
    }
}
