using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputReader : MonoBehaviour
    {
        [Header("Swipe Settings")]
        [SerializeField] private float swipeThreshold = 50f; // píxeles mínimos
        public bool InputEnabled { get; set; } = true;

        private PlayerInput _playerInput;
        private InputAction _primaryContact;  // tipo Button
        private InputAction _primaryPosition; // tipo Value (Vector2)

        private Vector2 _startPos;
        private bool _isSwiping;

        // Evento público para otros sistemas
        public delegate void SwipeAction(Vector2 startScreenPos, Vector2Int direction);
        public event SwipeAction OnSwipe;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            _primaryContact = _playerInput.actions["PrimaryContact"];
            _primaryPosition = _playerInput.actions["PrimaryPosition"];
        }

        private void OnEnable()
        {
            _primaryContact.started += OnTouchStart;
            _primaryContact.canceled += OnTouchEnd;
        }

        private void OnDisable()
        {
            _primaryContact.started -= OnTouchStart;
            _primaryContact.canceled -= OnTouchEnd;
        }

        private void OnTouchStart(InputAction.CallbackContext ctx)
        {
            if (!InputEnabled) return;
            _isSwiping = true;
            _startPos = _primaryPosition.ReadValue<Vector2>();
        }

        private void OnTouchEnd(InputAction.CallbackContext ctx)
        {
            if (!InputEnabled || !_isSwiping) return;
            _isSwiping = false;

            Vector2 endPos = _primaryPosition.ReadValue<Vector2>();
            Vector2 delta = endPos - _startPos;

            if (delta.magnitude < swipeThreshold)
                return; // swipe muy corto

            // Detectar dirección dominante
            Vector2Int dir;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                dir = new Vector2Int(delta.x > 0 ? 1 : -1, 0);
            else
                dir = new Vector2Int(0, delta.y > 0 ? 1 : -1);

            OnSwipe?.Invoke(_startPos, dir);
        }
    }

}
