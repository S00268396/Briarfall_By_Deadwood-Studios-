using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerLook : MonoBehaviour
{
    private Vector2 LookInput;
    private float MouseX;
    private float MouseY;
    private float XRotation;

    [Header("Settings")]
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private float HorizonSensitivity = 0.1f;
    [SerializeField] private float VerticalSensitivity = 0.1f;
    [SerializeField] private bool IsCursorVisible = true;

    private void Awake()
    {
        Cursor.visible = IsCursorVisible;

    }
    public void OnLook(InputAction.CallbackContext obj)
    {
        LookInput = obj.ReadValue<Vector2>();
        UpdateLook();
    }
    protected virtual void UpdateLook()
    {
        MouseX = LookInput.x * HorizonSensitivity;
        MouseY = LookInput.y * VerticalSensitivity;

        transform.Rotate(0f, MouseX, 0f);

        XRotation -= MouseY;
        XRotation = Mathf.Clamp(XRotation, -90f, 90f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(XRotation, 0f, 0f);
    }
    private void OnEnable()
    {
        InputManager.Actions.Game.Look.performed += OnLook;
    }
    private void OnDisable()
    {
        InputManager.Actions.Game.Look.performed -= OnLook;

    }

}

