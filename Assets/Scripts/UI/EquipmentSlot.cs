using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : SlotView
{
    public void Init(EquipmentView.EquipmentSlotData data) 
    {
        title.text = data.title;
    }
}
