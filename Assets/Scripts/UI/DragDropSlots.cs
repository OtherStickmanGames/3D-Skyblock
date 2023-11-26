using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class DragDropSlots : MonoBehaviour
{
    InventoryItem dragable;
    Vector3 itemHolderOriginPos;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData ped = new(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> hits = new();
            EventSystem.current.RaycastAll(ped, hits);

            var hit = hits.Find(h => h.gameObject.GetComponent<SlotView>());
            if(hit.gameObject)
            {
                var slot = hit.gameObject.GetComponent<SlotView>();
                //print(slot);
                if (slot && slot.Item != null)
                {
                    itemHolderOriginPos = slot.Item.view.transform.parent.localPosition;
                    dragable = slot.Item;
                }
            }
        }

        if(dragable != null)
        {
            var ScreenScale = transform.root.lossyScale.x;
            float k = transform.lossyScale.x / ScreenScale;
            var t = dragable.view.transform.parent;
            float x = (Input.mousePosition.x * k);// - ((Screen.width / 2) * k);
            float y = (Input.mousePosition.y * k);// - ((Screen.height / 2) * k);
            float z = t.position.z;
            t.position = new Vector3(x, y, z);
        }


    }
}
