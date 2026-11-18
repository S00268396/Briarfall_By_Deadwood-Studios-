using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
   private Vector3 lastCheckpoint;

   private void Start()
   {
       lastCheckpoint = transform.position;
   }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
        {
            RespawnAtCheckpoint();
        }
    }

    public void RespawnAtCheckpoint()
    {
        CharacterController controller = GetComponent<CharacterController>();
        controller.enabled = false;
        transform.position = lastCheckpoint;
        controller.enabled = true;
    }

    public void SetNewCheckpoint(Transform NewCheckpoint)
    {
        lastCheckpoint = NewCheckpoint.position;
        Debug.Log($"New Checkpoint Set: {lastCheckpoint}");
    }
}
