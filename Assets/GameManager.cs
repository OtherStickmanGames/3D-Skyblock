using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Player> players = new();
    [SerializeField] List<GameItemData> itemsData;

    public List<GameItemData> ItemsData => itemsData;

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

    public static void SetLayerByChild(GameObject go, int layer)
    {
        if (go.transform.childCount == 0)
            return;

        foreach (Transform t in go.transform)
        {
            t.gameObject.layer = layer;
            SetLayerByChild(t.gameObject, layer);
        }
    }
}
