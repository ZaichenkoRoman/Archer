using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    // ───── Serialized Fields ─────
    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActions;

    // ───── Events ─────
    public event Action OnTouchStart;
    public event Action OnTouchEnd;
    public event Action<Vector2> OnTouchPositionChanged;

    // ───── Private Fields ─────
    private InputAction _touchStart;
    private InputAction _touchEnd;
    private InputAction _touchPosition;

    private InputActionMap _gameplayMap;

    // ───── Unity Methods ─────
    private void OnEnable()
    {
        _gameplayMap = inputActions.FindActionMap("Gameplay");

        _touchStart = _gameplayMap.FindAction("TouchStart");
        _touchEnd = _gameplayMap.FindAction("TouchEnd");
        _touchPosition = _gameplayMap.FindAction("TouchPos");

        // Подписки
        _touchStart.performed += OnTouchStartPerformed;
        _touchEnd.performed += OnTouchEndPerformed;

        _gameplayMap.Enable();
    }

    private void OnDisable()
    {
        // Отписки
        _touchStart.performed -= OnTouchStartPerformed;
        _touchEnd.performed -= OnTouchEndPerformed;

        _gameplayMap.Disable();
    }

    private void Update()
    {
        if (_touchPosition != null && _touchPosition.enabled)
        {
            Vector2 pos = _touchPosition.ReadValue<Vector2>();
            OnTouchPositionChanged?.Invoke(pos);
        }
    }

    // ───── Event Handlers ─────
    private void OnTouchStartPerformed(InputAction.CallbackContext context)
    {
        OnTouchStart?.Invoke();
    }

    private void OnTouchEndPerformed(InputAction.CallbackContext context)
    {
        OnTouchEnd?.Invoke();
    }
}