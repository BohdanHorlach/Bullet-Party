using General.Player;
using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace General.Utils
{
    public class InputReader : MonoBehaviour
    {
        private const float DIRECTION_EPSILON = 0.0001f;

        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private GameObject _uiInput;

        private InputAction _moveAction;
        private InputAction _stickFireAction;
        private InputAction _mouseFireAction;
        private InputAction _mousePositionAction;

        private bool _isMouseFiring;
        private Vector2 _lastMouseDirection;

        public static Vector2 MoveDirection { get; private set; } = Vector2.zero;
        public static Action<Vector2> OnFire;


        private void Awake()
        {
            _moveAction = _playerInput.actions["Move"];
            _stickFireAction = _playerInput.actions["Fire"];
            _mouseFireAction = _playerInput.actions["MouseFire"];
            _mousePositionAction = _playerInput.actions["MousePosition"];
        }


        private void Start()
        {
            if (!Application.isMobilePlatform)
                _uiInput.SetActive(false);
        }


        private void OnEnable()
        {
            _moveAction.performed += OnInputMove;
            _moveAction.canceled += OnInputMove;

            _stickFireAction.performed += OnStickFireInput;
            _stickFireAction.canceled += OnStickFireCanceled;

            _mouseFireAction.started += OnMouseFireStarted;
            _mouseFireAction.canceled += OnMouseFireCanceled;
        }


        private void OnDisable()
        {
            _moveAction.performed -= OnInputMove;
            _moveAction.canceled -= OnInputMove;

            _stickFireAction.performed -= OnStickFireInput;
            _stickFireAction.canceled -= OnStickFireCanceled;

            _mouseFireAction.started -= OnMouseFireStarted;
            _mouseFireAction.canceled -= OnMouseFireCanceled;

            // не лишаємо "затиснутий" стан після вимкнення/зміни сцени
            if (_isMouseFiring)
                StopMouseFire();

            MoveDirection = Vector2.zero;
        }


        private void Update()
        {
            if (!_isMouseFiring)
                return;

            if (!TryGetMouseDirection(out Vector2 direction))
                return;

            // шлемо подію лише коли напрямок змінився (як і зі стіком)
            if ((direction - _lastMouseDirection).sqrMagnitude < DIRECTION_EPSILON)
                return;

            _lastMouseDirection = direction;
            Fire(direction);
        }


        private void OnInputMove(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();
        }


        private void Fire(Vector2 direction)
        {
            OnFire?.Invoke(direction.normalized);
        }


        private void OnStickFireInput(InputAction.CallbackContext context)
        {
            Fire(context.ReadValue<Vector2>());
        }


        private void OnStickFireCanceled(InputAction.CallbackContext context)
        {
            // якщо мишею стріляємо, стік не має зупиняти вогонь
            if (!_isMouseFiring)
                Fire(Vector2.zero);
        }


        private void OnMouseFireStarted(InputAction.CallbackContext context)
        {
            _isMouseFiring = true;
            _lastMouseDirection = Vector2.zero; // примусово відправити перший напрямок у наступному Update
        }


        private void OnMouseFireCanceled(InputAction.CallbackContext context)
        {
            StopMouseFire();
        }


        private void StopMouseFire()
        {
            _isMouseFiring = false;
            _lastMouseDirection = Vector2.zero;
            Fire(Vector2.zero);
        }


        private bool TryGetMouseDirection(out Vector2 direction)
        {
            direction = Vector2.zero;

            Camera cam = Camera.main;
            PlayerNetwork localPlayer = PlayerNetwork.Local;

            if (cam == null || localPlayer == null)
                return false;

            Vector2 screenPosition = _mousePositionAction.ReadValue<Vector2>();

            Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, Mathf.Abs(cam.transform.position.z));
            Vector2 worldPosition = cam.ScreenToWorldPoint(screenPoint);

            direction = worldPosition - (Vector2)localPlayer.transform.position;

            return direction.sqrMagnitude >= DIRECTION_EPSILON;
        }
    }
}