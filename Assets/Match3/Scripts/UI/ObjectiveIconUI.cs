using Level;
using ScriptableObjects.Level.Objetives;
using Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class ObjectiveIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ObjectiveBaseSO _objective;
        private TooltipUI _tooltip;

        public void Initialize(ObjectiveBaseSO objective, TooltipUI tooltip)
        {
            _objective = objective;
            _tooltip = tooltip;
            GetComponent<Image>().sprite = objective.Icon;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_objective == null || _tooltip == null) return;

            Vector2 mousePos = Vector2.zero;
            if (Mouse.current != null)
                mousePos = Mouse.current.position.ReadValue(); 

            _tooltip.Show(_objective.Name, _objective.GetResolvedDescription(ServiceLocator.Instance.Get<ObjectiveSystem>().Context), mousePos);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_tooltip == null) return;
            _tooltip.Hide();
        }
    }
}