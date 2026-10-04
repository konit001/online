using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleWalkthroughController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.8f;
    public float mouseSensitivity = 0.1f;
    public Transform cameraPivot;

    private CharacterController _controller;
    private float _pitch;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null) return;

        if (mouse != null)
        {
            Vector2 delta = mouse.delta.ReadValue() * mouseSensitivity;
            transform.Rotate(Vector3.up * delta.x);
            _pitch = Mathf.Clamp(_pitch - delta.y, -80f, 80f);
            if (cameraPivot != null)
                cameraPivot.localEulerAngles = new Vector3(_pitch, 0f, 0f);
        }

        float h = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
        float v = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
        float speed = moveSpeed * (keyboard.leftShiftKey.isPressed ? sprintMultiplier : 1f);

        Vector3 move = (transform.right * h + transform.forward * v) * speed;
        move.y = -2f;
        _controller.Move(move * Time.deltaTime);

        if (keyboard.escapeKey.wasPressedThisFrame)
            Cursor.lockState = CursorLockMode.None;
    }
}
