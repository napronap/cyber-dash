using UnityEngine;
using UnityEngine.InputSystem;
public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    private PlayerInputActions _playerInputActions;
    private void Awake()
    {
        //_playerInputActions.Player.Dash.performed += PlayerDash_performed;

        Instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
    }
    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }
    public Vector3 GetMousePosition()
    {
        Vector3 mousePos=Mouse.current.position.ReadValue();
        return mousePos;
    }
}

