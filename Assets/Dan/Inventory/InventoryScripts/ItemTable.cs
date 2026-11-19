using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTable : ScriptableObject
{
    public List<BaseTemData> items;

    public BaseTemData Find(string id)
    {
        foreach(var item in items)
        {
            if(item.ID == id)
            {
                return item;
            }    
        }
        return null;
    }
}
