using ScriptableObjects;
using UnityEngine;

namespace Core
{
    public class Gem : MonoBehaviour
    {
        [field: SerializeField]public GemSO GemSO {  get; private set; }

        public void SetGem(GemSO gemSO)
        {
            GemSO = gemSO;
            GetComponent<SpriteRenderer>().sprite = gemSO.Icon;
        }

        public GemSO GetGem() => GemSO;
    }

}