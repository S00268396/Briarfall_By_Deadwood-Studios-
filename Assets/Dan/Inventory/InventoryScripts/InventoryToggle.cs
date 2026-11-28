using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // drag your inventory here

    [Header("Player Scripts to Pause While Open")]
    public MonoBehaviour playerLook;
    public MonoBehaviour playerToolHandler;
    public MonoBehaviour playerMovement;

    private bool isOpen = false;

    void Start()
    {
        if( inventoryPanel != null )
        {
            inventoryPanel.SetActive( false ); //Ensure its starts closed
        }

        //Auto-find the player scripts on your Player object
        PlayerToolHandler handler = FindFirstObjectByType<PlayerToolHandler>();
        if( handler != null )
        {
            GameObject player = handler.gameObject;
            if (playerLook == null) playerLook = player.GetComponent("PlayerLook") as MonoBehaviour;
            if (playerToolHandler == null) playerToolHandler = handler;
            if(playerMovement == null) playerMovement = player.GetComponent("PlayerMovement") as MonoBehaviour;
        }
    }

    void Update()
    {
        //can add two inputs for the inventory open/closed
        if(Input.GetKeyDown(KeyCode.I))
        {
            isOpen = !isOpen;
            inventoryPanel.SetActive( isOpen );

            //Unlock the mouse cursor when inventory is open
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen? CursorLockMode.None : CursorLockMode.Locked;


            //Pause camera look, weapon swinging, and movement while inventory is open
            if (playerLook != null) playerLook.enabled = !isOpen;
            if (playerToolHandler != null) playerToolHandler.enabled = !isOpen;
            if (playerMovement != null) playerMovement.enabled = !isOpen;

            //Cancel any unfinished drag if you close the inventory mid-drag
            if(!isOpen)
            {
                InventoryManager invManager = FindFirstObjectByType<InventoryManager>();
                if (invManager != null)
                    invManager.CancelDrag();
            }
        }
    }
}
