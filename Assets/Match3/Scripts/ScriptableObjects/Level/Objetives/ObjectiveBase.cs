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

        /// <summary>
        /// Inicializa variables del contexto (se llama al comenzar el nivel)
        /// </summary>
        public abstract void Initialize(ObjectiveContext context);

        /// <summary>
        /// Devuelve el valor actual alcanzado para este objetivo (ej: 5/10 gemas)
        /// </summary>
        public abstract int GetCurrentValue(ObjectiveContext context);

        /// <summary>
        /// Verifica si el objetivo está completado.
        /// </summary>
        public virtual bool IsCompleted(ObjectiveContext context)
        {
            return GetCurrentValue(context) >= _targetValue;
        }

        /// <summary>
        /// Devuelve cuánto falta para cumplirlo (ej: 3 faltantes)
        /// </summary>
        public int GetRemaining(ObjectiveContext context)
        {
            return Mathf.Max(0, _targetValue - GetCurrentValue(context));
        }

        /// <summary>
        /// Devuelve el progreso normalizado entre 0 y 1
        /// </summary>
        public float GetProgress(ObjectiveContext context)
        {
            return Mathf.Clamp01((float)GetCurrentValue(context) / _targetValue);
        }
        public virtual string GetResolvedDescription(ObjectiveContext context)
        {
            if (context == null)
                return _description;

            string desc = _description;
           // desc = desc.Replace("{target}", _targetValue.ToString());
           // desc = desc.Replace("{current}", GetCurrentValue(context).ToString());
            desc = desc.Replace("{remaining}", GetRemaining(context).ToString());
            return desc;
        }
    }
}
