using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Item Table", menuName ="Items/Table")]
public class ItemTable : ScriptableObject
{
    [SerializeField] private List<BaseTemData> Items;

    public BaseTemData Find(string id)
    {
        return Items.Find(item => item.ID == id);
    }
}
