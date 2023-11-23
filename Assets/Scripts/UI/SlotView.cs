using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SlotView : MonoBehaviour
{
    [SerializeField] Transform itemParent;
    [SerializeField] TMP_Text txtCount;

    public InventoryItem Item { get; private set; }

    public void Init()
    {
        UpdateSlot();
    }


    public void SetItem(InventoryItem item)
    {
        item.view.transform.SetParent(itemParent, false);
        item.view.transform.localPosition = Vector3.zero;
        item.view.transform.localRotation = Quaternion.Euler(Rotation(item.ID));

        item.view.SetActive(true);
        item.view.layer = 5;

        foreach (var view in item.view.GetComponentsInChildren<Transform>())
        {
            view.gameObject.layer = 5;
        }

        txtCount.text = item.count > 1 ? $"x{item.count}" : "";

        Item = item;
    }

    public void UpdateSlot()
    {
        if (Item != null)
        {
            txtCount.text = Item.count > 1 ? $"x{Item.count}" : "";

            if(Item.count == 0)
            {
                foreach (Transform view in itemParent)
                {
                    Destroy(view.gameObject);
                }
                Item = null;
            }
        }
        else
        {
            txtCount.text = string.Empty;
        }
    }

    Vector3 Rotation(byte id)
    {
        //switch (id)
        //{
        //    case ITEMS.INGOT_IRON:
        //        return new(1.327f, 95.58f, -33.715f);
        //    case ITEMS.STICK:
        //        return new(-51f, 39f, 3.189f);
        //    case ITEMS.AXE_WOODEN:
        //        return new(18.385f, 208.087f, -41.801f);
        //}

        return Vector3.zero;
    }
}
