using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int CountQuickSlots = 8;

    List<InventoryItem> quick = new();

    Player player;

    public Inventory(Player player)
    {
        this.player = player;
    }

    public void AddItem(byte ID, GameObject view)
    {
        var newItem = new InventoryItem() 
        { 
            ID = ID, 
            view = view, 
            count = 1,
            isStackable = IsStackable(ID)
        };

        var item = quick.Find(i => i.ID == ID);
        if(item != null)
        {
            item.count++;
            Destroy(view);
        }
        else
        {
            quick.Add(newItem);
        }

        EventsHolder.onQuickInventoryAddItem?.Invoke(player, newItem);
    }

    bool IsStackable(byte ID)
    {
        return true;
    }
}
