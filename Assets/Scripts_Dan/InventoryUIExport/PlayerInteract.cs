using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask itemLayer;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUp();
        }
    }

    void TryPickUp()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        //check to see if the item has the itempick script
        if (Physics.Raycast(ray, out hit, interactRange, itemLayer))
        {
            ItemPickup item = hit.collider.GetComponent<ItemPickup>();

            if (item != null)
            {
                item.CollectItem();
            }
        }
    }
}
