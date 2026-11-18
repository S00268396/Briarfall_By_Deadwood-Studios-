using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [Header("UI Components")]
    public Image iconDisplay; // drag the image component here

    //Update the slot with a new item
    public void UpdateSlot(InventoryItem item)
    {
        if (iconDisplay == null)
        {
            Debug.LogWarning($"Slot on {gameObject.name} is missing its icon display reference!");
            return;
        }
        if (item == null || item.Item == null)
        {
            ClearSlot();
            return;
        }

        //turn oon the image and set the icon
        if(item.Item != null)
        {
            iconDisplay.sprite = item.Item.Icon;
            iconDisplay.enabled = true;
        }
        else
        {
            iconDisplay.enabled = false;
        }
    }

    public void ClearSlot()
    {
        if(iconDisplay != null) 
        {
           iconDisplay.sprite = null;
           iconDisplay.enabled = false;
        }
    }
}
