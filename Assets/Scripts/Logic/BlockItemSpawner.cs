
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockItemSpawner : IUpdateble
{
    DropedBlockGenerator blockGenerator = new();
    List<SpawnedBlockData> spawnedBlocks = new();
    List<SpawnedBlockData> removable = new();

    public BlockItemSpawner()
    {
        EventsHolder.onBlockPicked.AddListener(Block_Picked);
    }

    public void Update()
    {
        foreach (var block in spawnedBlocks)
        {
            block.view.transform.Rotate(Vector3.up, 1f);
            block.lifetime += Time.deltaTime;

            foreach (var player in GameManager.Inst.players)
            {
                CheckDistanceToPlayer(player, block);
            }
        }

        foreach (var item in removable)
        {
            spawnedBlocks.Remove(item);
        }
        removable.Clear();
    }

    void CheckDistanceToPlayer(Player player, SpawnedBlockData data)
    {
        if (data.lifetime > 1f)
        {
            var dist = Vector3.Distance(player.transform.position, data.view.transform.position);

            if (dist < 3 && player.inventory.AvailabelTake(data))
            {
                var dir = player.transform.position - data.view.transform.position;
                dir.Normalize();

                if (dist < 0.18f)
                {
                    TakeBlock(player, data);   
                }
                else
                {
                    data.view.transform.position += (5 / dist) * Time.deltaTime * dir;
                }
            }
        }
    }

    void TakeBlock(Player player, SpawnedBlockData block)
    {
        player.inventory.AddItem(block.ID, block.view);
        removable.Add(block);

        EventsHolder.onItemPicked?.Invoke(player, block);
    }

    private void Block_Picked(BlockPickedData data)
    {
        if (data.ID == 2)
        {
            data.ID = 3;
        }

        var dropedBlock = new GameObject($"Droped Block - {data.ID}");
        dropedBlock.AddComponent<MeshRenderer>().material = WorldGenerator.Inst.mat;
        dropedBlock.AddComponent<MeshFilter>().mesh = blockGenerator.GenerateMesh(data.ID);
        dropedBlock.transform.localScale /= 3f;

        float offsetRandomX = Random.Range(0.3f, 0.57f);
        float offsetRandomY = Random.Range(0.3f, 0.57f);
        float offsetRandomZ = Random.Range(0.3f, 0.57f);

        dropedBlock.transform.position = data.pos + new Vector3(offsetRandomX, offsetRandomY, offsetRandomZ);

        spawnedBlocks.Add(new() { ID = data.ID, view = dropedBlock });
    }

   
}

public class SpawnedBlockData
{
    public byte ID;
    public GameObject view;
    public float lifetime;
}

public class BlockPickedData
{
    public byte ID;
    public Vector3 pos;
}
