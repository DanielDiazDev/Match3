using Core;
using ScriptableObjects;
using UnityEngine;


namespace Systems
{
    public class GemFactory
    {
        private readonly Transform _parent;

        public GemFactory(Transform parent)
        {
            _parent = parent;
        }
        public IGem Create(GemSO gemSO, Vector2 position, Quaternion rotation)
        {
            return UnityEngine.Object.Instantiate(gemSO.Prefab, position, rotation).GetComponent<IGem>();
        }
        
    }
}
