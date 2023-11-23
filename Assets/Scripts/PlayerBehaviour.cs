using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldGenerator;


public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] Transform blockHighlightPrefab;
    [SerializeField] LayerMask layerMask;

    Transform blockHighlight;
    Player player;

    private void Start()
    {
        blockHighlight = Instantiate(blockHighlightPrefab, Vector3.zero, Quaternion.identity);

        player = GetComponent<Player>();
        EventsHolder.playerSpawnedMine?.Invoke(player);
    }

    private void Update()
    {
        BlockRaycast();
    }

    void BlockRaycast()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 8f, layerMask))
        {
            blockHighlight.position = Vector3.zero;

            Vector3 normalPos = hit.point - (hit.normal / 2);

            int x = Mathf.FloorToInt(normalPos.x);
            int y = Mathf.FloorToInt(normalPos.y);
            int z = Mathf.FloorToInt(normalPos.z);

            Vector3 blockPosition = new(x, y, z);

            blockHighlight.position = blockPosition;
            //blockHighlight.forward = Vector3.forward;

            if (Input.GetMouseButtonDown(0))
            {
                var generator = WorldGenerator.Inst;
                var fixedPos = blockPosition + Vector3.right;

                var chunck = generator.GetChunk(fixedPos);
                var pos = chunck.renderer.transform.position;

                int xBlock = (int)(blockPosition.x - pos.x) + 1;
                int yBlock = (int)(blockPosition.y - pos.y);
                int zBlock = (int)(blockPosition.z - pos.z);

                byte blockID = chunck.blocks[xBlock, yBlock, zBlock];
                chunck.blocks[xBlock, yBlock, zBlock] = 0;

                var mesh = generator.UpdateMesh(chunck);//, (int)pos.x, (int)pos.y, (int)pos.z);
                chunck.meshFilter.mesh = mesh;
                chunck.collider.sharedMesh = mesh;

                for (int p = 0; p < 6; p++)
                {
                    var blockPos = new Vector3(xBlock, yBlock, zBlock);

                    Vector3 checkingBlockPos = blockPos + World.faceChecks[p];
                    var blockInOtherChunckPos = checkingBlockPos + pos;

                    
                    if (!IsBlockChunk((int)checkingBlockPos.x, (int)checkingBlockPos.y, (int)checkingBlockPos.z))
                    {
                        var otherChunck = generator.GetChunk(checkingBlockPos + pos);

                        var otherMesh = generator.UpdateMesh(otherChunck);
                        otherChunck.meshFilter.mesh = otherMesh;
                        otherChunck.collider.sharedMesh = otherMesh;
                    }
                }


                EventsHolder.onBlockPicked?.Invoke(new() { ID = blockID, pos = blockPosition });                //WorldHit(ref chunck, ref hitComponent);

                //entityBlockHit = godcraft.EcsWorld.NewEntity();

                //var pool = godcraft.EcsWorld.GetPool<ChunckHitEvent>();
                //pool.Add(characterEntity);
                //ref var component = ref pool.Get(characterEntity);
                //component.collider = hit.collider;
                //component.position = blockPosition;
                //component.blockId = 0;

                //onChunkHit?.Invoke(new Entity { id = characterEntity }, component);

                //isHit = true;
            }

            PlaceBlock(blockPosition + hit.normal);

            //if (Input.GetMouseButtonUp(0) && isHit)
            //{
            //    isHit = false;

            //    var pool = godcraft.EcsWorld.GetPool<ChunckHitEvent>();
            //    var filter = godcraft.EcsWorld.Filter<ChunckHitEvent>().End();
            //    foreach (var entity in filter)
            //    {
            //        if (entity == characterEntity)
            //        {
            //            pool.Del(characterEntity);
            //        }
            //    }

            //}

            //if (Input.GetMouseButtonDown(1))
            //{
            //    // �����-�� ����� ���������� 1 �� ��� X, �� ������ ���, �� ������ ��� ��������
            //    ref var chunck = ref Service<World>.Get().GetChunk(blockPosition + Vector3.right);
            //    var pos = chunck.renderer.transform.position;

            //    // �����-�� ����� ���������� 1 �� ��� X, �� ������ ���, �� ������ ��� ��������
            //    int xBlock = x - Mathf.FloorToInt(pos.x) + 1;
            //    int yBlock = y - Mathf.FloorToInt(pos.y);
            //    int zBlock = z - Mathf.FloorToInt(pos.z);
            //    byte hitBlockID = chunck.blocks[xBlock, yBlock, zBlock];

            //    if (hitBlockID == 100 || hitBlockID == 101 || hitBlockID == 102)
            //    {
            //        GlobalEvents.interactBlockHited.Invoke(hitBlockID, new(x + 1, y, z));
            //    }
            //    else
            //    {
            //        int idx = 0;
            //        foreach (var entity in filter)
            //        {
            //            if (idx == InputHandler.Instance.quickSlotID - 1)
            //            {
            //                var poolItems = ecsWorld.GetPool<InventoryItem>();
            //                ref var item = ref poolItems.Get(entity);

            //                if (item.itemType == ItemType.Block)
            //                {
            //                    var e = godcraft.EcsWorld.NewEntity();

            //                    var pool = godcraft.EcsWorld.GetPool<ChunckHitEvent>();
            //                    pool.Add(e);
            //                    ref var component = ref pool.Get(e);
            //                    component.collider = hit.collider;
            //                    component.position = blockPosition + hit.normal;
            //                    component.blockId = item.blockID;

            //                    onChunkHit?.Invoke(new Entity { id = e }, component);
            //                    GlobalEvents.onBlockPlaced?.Invoke(item.blockID, blockPosition + hit.normal);

            //                    // HOT FIX ������� � ��������� �������
            //                    item.count--;
            //                    if (item.count == 0)
            //                    {
            //                        Destroy(item.view);
            //                        ecsWorld.DelEntity(entity);
            //                    }

            //                    StartCoroutine(Delay());

            //                    //-----------------------------------
            //                }
            //                else
            //                {
            //                    ref var used = ref ecsWorld.GetPool<ItemUsed>().Add(entity);
            //                    used.entity = entity;
            //                    used.id = item.blockID;

            //                    StartCoroutine(Delay());
            //                }

            //                IEnumerator Delay()
            //                {
            //                    yield return null;

            //                    GlobalEvents.itemUsing?.Invoke(entity);
            //                }

            //                break;
            //            }
            //            idx++;
            //        }


            //    }
            //}
        }
        else
        {
            blockHighlight.position = default;
        }
    }

    void PlaceBlock(Vector3 blockPosition)
    {
        if (Input.GetMouseButtonDown(1) && player.inventory.CurrentSelectedItem != null)
        {
            var item = player.inventory.CurrentSelectedItem;
            // �����-�� ����� ���������� 1 �� ��� X, �� ������ ���, �� ������ ��� ��������
            var generator = WorldGenerator.Inst;
            var chunck = generator.GetChunk(blockPosition + Vector3.right);
            var pos = chunck.renderer.transform.position;

            int xBlock = (int)(blockPosition.x - pos.x) + 1;
            int yBlock = (int)(blockPosition.y - pos.y);
            int zBlock = (int)(blockPosition.z - pos.z);
            // �����-�� ����� ���������� 1 �� ��� X, �� ������ ���, �� ������ ��� ��������
            byte hitBlockID = chunck.blocks[xBlock, yBlock, zBlock];

            chunck.blocks[xBlock, yBlock, zBlock] = item.ID;

            var mesh = generator.UpdateMesh(chunck);//, (int)pos.x, (int)pos.y, (int)pos.z);
            chunck.meshFilter.mesh = mesh;
            chunck.collider.sharedMesh = mesh;

            for (int p = 0; p < 6; p++)
            {
                var blockPos = new Vector3(xBlock, yBlock, zBlock);

                Vector3 checkingBlockPos = blockPos + World.faceChecks[p];
                var blockInOtherChunckPos = checkingBlockPos + pos;


                if (!IsBlockChunk((int)checkingBlockPos.x, (int)checkingBlockPos.y, (int)checkingBlockPos.z))
                {
                    var otherChunck = generator.GetChunk(checkingBlockPos + pos);

                    var otherMesh = generator.UpdateMesh(otherChunck);
                    otherChunck.meshFilter.mesh = otherMesh;
                    otherChunck.collider.sharedMesh = otherMesh;
                }
            }
            //print(item);
            player.inventory.RemoveItem(item);

        }
    }

    bool IsBlockChunk(int x, int y, int z)
    {
        if (x < 0 || x > size - 1 || y < 0 || y > size - 1 || z < 0 || z > size - 1)
            return false;
        else
            return true;
    }
}
