using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class TooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private CanvasGroup _canvasGroup;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup.blocksRaycasts = false;
            Hide();
        }

        public void Show(string title, string description, Vector2 position)
        {
            _titleText.text = title;
            _descriptionText.text = description;
            _rectTransform.position = position;
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (gameObject.activeSelf && Mouse.current != null)
            {
                _rectTransform.position = Mouse.current.position.ReadValue() + new Vector2(16f, -16f);
            }
        }
    }
}