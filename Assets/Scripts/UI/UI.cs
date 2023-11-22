using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] HUD HUD;

    Player mine;

    private void Awake()
    {
        EventsHolder.playerSpawnedMine.AddListener(PlayerMine_Spawned);
    }

    private void PlayerMine_Spawned(Player mine)
    {
        HUD.Init(mine);
    }
}
