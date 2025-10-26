

using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GemSO", menuName = "Pieces/GemSO")]
    public class GemSO : ScriptableObject
    {
        [field: SerializeField]public Sprite Icon {  get;  set; }
        [field: SerializeField]public GameObject Prefab {  get;  set; }
        [field: SerializeField] public bool IsPowerUp { get; private set; } = false;
    }
  
   

}
