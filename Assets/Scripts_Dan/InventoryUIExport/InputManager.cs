using UnityEngine;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    public static ApplicationActions Actions { get; private set; }

    static InputManager()
    {
        Actions = new ApplicationActions();
        Actions.Enable();
    }
}
