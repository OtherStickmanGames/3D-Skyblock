using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentSlot : SlotView
{

    ItemPurpose itemPurpose;

    public ItemPurpose Purpose => itemPurpose;

    public UnityEvent<InventoryItem> onEquip;

    public void Init(EquipmentView.EquipmentSlotData data) 
    {
        title.text = data.title;
        itemPurpose = data.itemPurpose;
    }

    public override void SetItem(InventoryItem item)
    {
        base.SetItem(item);

        onEquip?.Invoke(item);
    }
}
