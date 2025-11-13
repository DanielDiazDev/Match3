using Level;
using UnityEngine;

namespace ScriptableObjects.Level.Objetives
{
    public abstract class ObjectiveBaseSO : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _description;
        public string Description => _description;
        public Sprite Icon => _icon;

        public abstract void Initialize(ObjectiveContext context);
        public abstract bool IsCompleted(ObjectiveContext context);
    }
}