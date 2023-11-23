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
        EventsHolder.onQuickInventoryRemoveItem.AddListener(QuickInventoryItem_Removed);

        CreateSlots();
    }

    private void Update()
    {
        KeyboardInput();
    }

    private void QuickInventoryItem_Added(Player player, InventoryItem item)
    {
        if (player != owner)
            return;

        var slot = slots.Find(s => s.Item != null && s.Item == item);
        //print(slot);
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

    private void QuickInventoryItem_Removed(Player player, InventoryItem item)
    {
        if (player != owner)
            return;

        var slot = slots.Find(s => s.Item != null && s.Item == item);
        slot.UpdateSlot();
    }

    void CreateSlots()
    {
        Clear();

        for (int i = 0; i < owner.inventory.CountQuickSlots; i++)
        {
            var view = Instantiate(slotViewPrefab, parent);
            view.name = view.name.Insert(0, $"{i}:");
            view.Init();
            slots.Add(view);
        }
    }

    void SelectSlot(SlotView slot)
    {
        EventsHolder.onQuickInventoryItemSelect.Invoke(owner, slot.Item);
    }

    

    void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSlot(slots[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSlot(slots[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSlot(slots[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectSlot(slots[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectSlot(slots[4]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectSlot(slots[5]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectSlot(slots[6]);
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


