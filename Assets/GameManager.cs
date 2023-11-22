using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Player> players = new();

    public static GameManager Inst;

    private void Awake()
    {
        Inst = this;

        EventsHolder.onPlayerSpawnAny.AddListener(PlayerAny_Spawned);
    }

    private void Start()
    {
        
    }

    private void PlayerAny_Spawned(Player player)
    {
        players.Add(player);
    }
}
