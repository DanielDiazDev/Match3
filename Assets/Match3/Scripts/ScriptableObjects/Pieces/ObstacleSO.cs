using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ObstacleSO", menuName = "Pieces/ObstacleSO")]
    public class ObstacleSO : ScriptableObject
    {
        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public BiomeType BiomeType { get; private set; }
        [field: SerializeField] public int HitsRequired { get; private set; } = 1; // Número de matches necesarios para destruir
    }
}

