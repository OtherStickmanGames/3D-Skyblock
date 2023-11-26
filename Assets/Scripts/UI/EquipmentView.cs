using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentView : MonoBehaviour
{
    [SerializeField] EquipmentSlot equipmentSlotPrefab;
    [SerializeField] Transform parent;
    [SerializeField] List<EquipmentSlotData> slotsData;

    Player owner;

    public void Init(Player owner)
    {
        this.owner = owner;

        CreateSlots();
    }

    void CreateSlots()
    {
        UI.ClearParent(parent);

        foreach (var data in slotsData)
        {
            var slotView = Instantiate(equipmentSlotPrefab, parent);
            slotView.Init(data);
        }
    }


    [System.Serializable]
    public class EquipmentSlotData
    {
        public string title;
        
    }
}
