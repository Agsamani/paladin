using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    private InputSystem_Actions _input;
    private InputAction _look;
    private InputAction _move;

    private CharacterController _controller;

    public float LookSense = 10f;
    public float MoveSense = 10f;

    public Transform LookCamera;

    private Vector2 _rot;


    private void Awake()
    {
        _input = new InputSystem_Actions();
        _look = _input.Player.Look;
        _move = _input.Player.Move;

        _controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Look
        Vector2 look = _look.ReadValue<Vector2>() * Time.deltaTime * LookSense;
        _rot.y += look.x;
        _rot.x -= look.y;
        _rot.x = Mathf.Clamp(_rot.x, -60.0f, 60.0f);

        LookCamera.localRotation = Quaternion.Euler(_rot.x, 0, 0);
        transform.rotation = Quaternion.Euler(0, _rot.y, 0);

        // Move
        Vector2 move = _move.ReadValue<Vector2>() * MoveSense;
        Vector3 dir = transform.right * move.x + transform.forward * move.y;
        _controller.SimpleMove(dir);
    }


    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

}
