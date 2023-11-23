using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;

    private void Awake()
    {
        inventory = new(this);

    }

    private void Start()
    {
        EventsHolder.onPlayerSpawnAny?.Invoke(this);
    }
}
