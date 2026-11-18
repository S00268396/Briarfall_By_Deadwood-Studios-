using UnityEngine;
using UnityEngine.InputSystem;


[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public CharacterController Controller;

    

    public bool IsCrouching { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsSprinting { get; private set; }
    public Vector3 Velocity { get { return Controller.velocity; } }

    private Vector3 MovementInput;
    private Vector3 MovementVelocity;
    private Vector3 Movement;
    private float CurrentSpeed;

    [Header("Settings")]
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float SprintSpeed;
    [SerializeField] private float CrouchSpeed;
    [SerializeField] private float CrouchHeight;
    [SerializeField] private float JumpHeight;
    [SerializeField] private float Gravity = 9.81f;
    private float OriginalHeight;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        OriginalHeight = Controller.height;
        CurrentSpeed = WalkSpeed;
    }
    private void OnEnable()
    {
        InputManager.Actions.Game.Move.performed += OnMove;
        InputManager.Actions.Game.Move.canceled += OnMove;
        InputManager.Actions.Game.Jump.performed += OnJump;
        InputManager.Actions.Game.Crouch.performed += OnCrouch;
        InputManager.Actions.Game.Sprint.performed += OnSprint;
    }
    private void OnDisable()
    {
        InputManager.Actions.Game.Move.performed -= OnMove;
        InputManager.Actions.Game.Move.canceled -= OnMove;
        InputManager.Actions.Game.Jump.performed -= OnJump;
        InputManager.Actions.Game.Crouch.performed -= OnCrouch;
        InputManager.Actions.Game.Sprint.performed -= OnSprint;
    }
    public void OnMove(InputAction.CallbackContext obj)
    {
        MovementInput = obj.ReadValue<Vector2>();
        Debug.Log("Walking");
    }
    public bool CanJump()
    {
        return Controller.isGrounded;        
    }
    public void OnJump(InputAction.CallbackContext obj)
    {
        if (CanJump())
        {
            MovementVelocity.y = JumpHeight;
        }
    }
    public void OnCrouch(InputAction.CallbackContext obj)
    {
        IsCrouching = !IsCrouching;

        Controller.height = IsCrouching ? CrouchHeight : OriginalHeight;
        CurrentSpeed = IsCrouching ? CrouchSpeed : WalkSpeed;
        Debug.Log("Crouching");

    }
    public void OnSprint(InputAction.CallbackContext obj)
    {
        IsSprinting = !IsSprinting;

        if (!IsCrouching)
        {
            CurrentSpeed = IsSprinting ? SprintSpeed : WalkSpeed;
        }
        Debug.Log("Sprinting");
    }

    private void Update()
    {
        if (Controller.isGrounded && MovementVelocity.y < 0f)
        {
            MovementVelocity.y = -2f;
        }

        Movement = transform.right * MovementInput.x + transform.forward * MovementInput.y;

        MovementVelocity.y -= Gravity * Time.deltaTime;


        Vector3 totalMovement = (Movement * CurrentSpeed) + MovementVelocity;


        Controller.Move(totalMovement * Time.deltaTime);
    }    
}
