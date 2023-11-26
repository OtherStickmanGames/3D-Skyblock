using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public string name;
    public byte ID;
    public GameObject view;
    public bool isStackable;
    public int stackSize;

    public int count;
}

[System.Serializable]
public class GameItemData : InventoryItem
{
    public InventoryItem CreateItem()
    {
        return new()
        {
            ID = ID,
            name = name,
            view = Object.Instantiate(view, Vector3.zero, Quaternion.identity),
            isStackable = isStackable,
            stackSize = stackSize,
            count = count
        };
    }
}
