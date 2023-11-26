using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ITEMS;
using static BLOCKS;

public class Player : MonoBehaviour
{
    [SerializeField] Transform spineItemHolder;

    public Transform SpineItemHolder => spineItemHolder;

    public Inventory inventory;

    private void Awake()
    {
        inventory = new(this);

    }

    private void Start()
    {
        EventsHolder.onPlayerSpawnAny?.Invoke(this);

        var jetpack = GameManager.Inst.ItemsData.Find(i => i.ID == JETPACK);
        var item = jetpack.CreateItem();
        inventory.AddItem(item);
        
    }
}
