using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotsView : MonoBehaviour
{
    [SerializeField] SlotView slotViewPrefab;
    [SerializeField] Transform parent;

    List<SlotView> slots = new();

    Player owner;


    public void Init(Player owner)
    {
        this.owner = owner;

        EventsHolder.onQuickInventoryAddItem.AddListener(QuickInventoryItem_Added);

        CreateSlots();
    }

    private void QuickInventoryItem_Added(Player player, InventoryItem item)
    {
        if (player != owner)
            return;

        var slot = slots.Find(s => s.Item != null && s.Item.ID == item.ID);
        if (slot != null)
        {
            slot.UpdateSlot();
        }
        else
        {
            slot = slots.Find(s => s.Item == null);
            if (slot != null)
            {
                slot.SetItem(item);
            }
        }
    }

   
    void CreateSlots()
    {
        Clear();

        for (int i = 0; i < owner.inventory.CountQuickSlots; i++)
        {
            var view = Instantiate(slotViewPrefab, parent);
            view.Init();
            slots.Add(view);
        }
    }

    void Clear()
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
    }
}
