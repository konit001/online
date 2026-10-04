using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class userInput : NetworkBehaviour
{
    public InputActionAsset inputActions;
    public Vector2 _moveInput;
    public Vector2 _lookInput;
    public bool _jumpInput;
    public bool _interactInput;
    public bool _sprintInput;
    public bool _attackInput;
    public bool _attackReleased;
    private InputAction move;
    private InputAction look;
    private InputAction Jump;
    private InputAction interact;
    private InputAction sprint;
    private InputAction attack;

    public bool lockCursor;
    private bool hasJoined;
    void Update()
    {
        var playerInput = inputActions.FindActionMap("Player");
        startInputSystem(playerInput);
        updateInputSystem();
    }
    void startInputSystem(InputActionMap player)
    {
        move = player.FindAction("Move");
        look = player.FindAction("Look");
        Jump = player.FindAction("Jump");
        interact = player.FindAction("Interact");
        sprint = player.FindAction("Sprint");
        attack = player.FindAction("Attack");
    }
    void updateInputSystem()
    {
        _moveInput = move.ReadValue<Vector2>();
        _lookInput = look.ReadValue<Vector2>();
        if (Jump.WasPressedThisFrame()) _jumpInput = true;
        _interactInput = interact.WasReleasedThisFrame();
        _sprintInput = sprint.IsPressed();
        _attackInput = attack.IsPressed();
        _attackReleased = attack.WasReleasedThisFrame();
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            hasJoined = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnApplicationFocus(bool focus)
    {
        if (!hasJoined) return;

        lockCursor = focus;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();
}
