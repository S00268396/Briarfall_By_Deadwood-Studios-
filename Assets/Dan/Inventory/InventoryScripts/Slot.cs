using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public Image iconDisplay; // optional: found automatically from the child named "Icon"

    [HideInInspector] public int slotIndex;     //assigned automatically by InventoryManager
    [HideInInspector] public bool isHovering;   //True while mouse is over this slot

    bool resolved;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    // Always use THIS slot's own child called "Icon", whatever the Inspector field says.
    void ResolveIcon()
    {
        if (resolved) return;
        resolved = true;

        Transform child = transform.Find("Icon");
        if (child != null)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
                iconDisplay = img;
        }
    }

    // Update the slot with a new item
    public void UpdateSlot(InventoryItem item)
    {
        ResolveIcon();

        if (iconDisplay == null)
        {
            Debug.LogWarning($"{name} has no child called 'Icon' with an Image component!", this);
            return;
        }

        if (item == null || item.Item == null)
        {
            ClearSlot();
            return;
        }

        if (item.Item.Icon == null)
            Debug.LogWarning($"{item.Item.name} has no Icon sprite assigned!", item.Item);

        iconDisplay.sprite = item.Item.Icon;
        iconDisplay.enabled = item.Item.Icon != null;

        // Click this line in the Console to highlight the Image it just drew on
        Debug.Log($"{name} <- {item.Item.name} (drawing on {GetPath(iconDisplay.transform)})", iconDisplay);
    }

    public void ClearSlot()
    {
        ResolveIcon();

        if (iconDisplay == null) return;

        iconDisplay.sprite = null;
        iconDisplay.enabled = false;
    }

    static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}