using ScriptableObjects;
using UnityEngine;

namespace Core
{
    public interface IGem
    {
        void Init(GemSO gemSO);
        Transform Transform { get; }
        GemSO GetGem();
    }
    public class Gem : MonoBehaviour, IGem
    {
        [field: SerializeField]public GemSO GemSO {  get; private set; }

        public Transform Transform => transform;


        public void Init(GemSO gemSO)
        {
            GemSO = gemSO;
            GetComponent<SpriteRenderer>().sprite = gemSO.Icon;
        }

        public GemSO GetGem() => GemSO;
    }
    
}