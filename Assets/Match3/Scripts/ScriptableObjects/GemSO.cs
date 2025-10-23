

using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GemSO", menuName = "Pieces/GemSO")]
    public class GemSO : ScriptableObject
    {
        [field: SerializeField]public Sprite Icon {  get; private set; }
    }
}
