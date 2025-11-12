using Level;
using UnityEngine;

namespace ScriptableObjects.Level.Objetives
{
    public abstract class ObjectiveBaseSO : ScriptableObject
    {
        [SerializeField] private string _description;
        public string Description => _description;

        public abstract void Initialize(ObjectiveContext context);
        public abstract bool IsCompleted(ObjectiveContext context);
    }
}