using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Base")]
public class BaseTemData : ScriptableObject
{
    public string ID;
    public string Name = "Item Name";

    [TextArea(3, 5)]    //Makes the description box much larger in the unity Inspector
    public string Description = "Item Description";

  //  public ItemType Type = ItemType.Consumable; //Added the variable so you can actually assign the type!
  //  public ItemRarity Rarity = ItemRarity.Common;
    public int MaxStackSize = 999;
    public float Value = 0;
    public float Weight = 0;
    public Sprite Icon;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(string.IsNullOrEmpty(ID))
        {
            ID = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}

public enum ItemType
{
    Note,
    Stick,
    Rock
}