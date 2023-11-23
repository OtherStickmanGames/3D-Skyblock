using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public int CountQuickSlots = 8;

    public InventoryItem CurrentSelectedItem { get; set; }

    List<InventoryItem> quick = new();
    Player player;
    int stackSize = 8;

    public Inventory(Player player)
    {
        this.player = player;

        EventsHolder.onQuickInventoryItemSelect.AddListener(Item_Selected);

    }

    private void Item_Selected(Player owner, InventoryItem item)
    {
        CurrentSelectedItem = item;
        Debug.Log(item);
    }

    //private void Item_Selected(InventoryItem item)
    //{
    //    CurrentSelectedItem = item;
    //    print(item);
    //}

    public void AddItem(byte ID, GameObject view)
    {
        var item = quick.Find(i => i.ID == ID && i.count < i.stackSize && i.isStackable);
        if(item != null)
        {
            item.count++;
            if (item.count > 1)
                Object.Destroy(view);

            EventsHolder.onQuickInventoryAddItem?.Invoke(player, item);
        }
        else
        {
            var newItem = new InventoryItem()
            {
                ID = ID,
                view = view,
                count = 1,
                stackSize = stackSize,
                isStackable = IsStackable(ID)
            };

            quick.Add(newItem);
            EventsHolder.onQuickInventoryAddItem?.Invoke(player, newItem);
        }        
    }

    public void RemoveItem(InventoryItem item)
    {
        var founded = quick.Find(i => i == item);
        founded.count--;

        EventsHolder.onQuickInventoryRemoveItem?.Invoke(player, item);

        if(founded.count == 0)
        {
            if (CurrentSelectedItem == item)
                CurrentSelectedItem = null;

            quick.Remove(item);
        }


    }

    public bool AvailabelTake(SpawnedBlockData blockData)
    {
        if (quick.Count < CountQuickSlots)
            return true;
        else
        {
            var ID = blockData.ID;
            var item = quick.Find(i => i.ID == ID && i.isStackable && i.count < i.stackSize);
            if(item != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    bool IsStackable(byte ID)
    {
        return true;
    }
}
