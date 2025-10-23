

using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GemSO", menuName = "Pieces/GemSO")]
    public class GemSO : ScriptableObject
    {
        [field: SerializeField]public Sprite Icon {  get; private set; }
        [field: SerializeField]public GameObject Prefab {  get; private set; }
        [field: SerializeField]public GemType Type {  get; private set; }
    }
    public enum GemType
    {
        Normal,
        LineHorizontal,
        LineVertical,
        Bomb,
        GemClear
    }
}
