using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public ProceduralGeneration procedural;
    public Material mat;

    public Dictionary<Vector3Int, ChunckComponent> chuncks = new();

    public const int size = 32;
    public const int noiseScale = 100;
    public const float TextureOffset = 1f / 16f;
    public const float landThresold = 0.11f;
    public const float smallRockThresold = 0.8f;

    Dictionary<BlockSide, List<Vector3>> blockVerticesSet;
    Dictionary<BlockSide, List<int>> blockTrianglesSet;

    readonly List<Vector3> vertices = new();
    readonly List<int> triangulos = new();
    readonly List<Vector2> uvs = new();

    List<Player> players = new();

    public static WorldGenerator Inst { get; set; }

    private void Start()
    {
        Inst = this;

        Application.targetFrameRate = 60;

        EventsHolder.onPlayerSpawnAny.AddListener(PlayerAny_Spawned);

        foreach (var item in FindObjectsOfType<Player>())
        {
            players.Add(item);
        }

        blockVerticesSet = new Dictionary<BlockSide, List<Vector3>>();
        blockTrianglesSet = new Dictionary<BlockSide, List<int>>();

        DictionaryInits();
        InitTriangulos();

        //chuncks = new ChunckComponent[size, 1, size];

        //for (int x = 0; x < size * worldSize; x += size)
        //{
        //    for (int z = 0; z < size * worldSize; z += size)
        //    {
        //        int xIdx = Mathf.FloorToInt((float)x / size);
        //        int zIdx = Mathf.FloorToInt((float)z / size);

        //        chuncks[xIdx, 0, zIdx] = CreateChunck(x, 0, z);
        //    }
        //}

        //Debug.Log(minValue + " --- " + maxValue);
    }

    private void PlayerAny_Spawned(Player player)
    {
        players.Add(player);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

            
        }

        DynamicCreateChunck();
        
    }

    void DynamicCreateChunck()
    {
        var viewDistance = 3 * size;

        foreach (var player in players)
        {
            var pos = player.transform.position;

            var primary = GetChunk(pos + (Vector3.down * 88), out var key);
            if (primary == null)
            {
                key *= size;
                CreateChunck(key.x, key.y, key.z);
                return;
            }
          

            for (float x = -viewDistance + pos.x; x < viewDistance + pos.x; x += size)
            {
                for (float y = -viewDistance + pos.y; y < viewDistance + pos.y; y += size)
                {
                    for (float z = -viewDistance + pos.z; z < viewDistance + pos.z; z += size)
                    {
                        var checkingPos = new Vector3(x, y, z);
                        var chunck = GetChunk(checkingPos, out var chunckKey);

                        if (chunck == null)
                        {
                            chunckKey *= size;
                            CreateChunck(chunckKey.x, chunckKey.y, chunckKey.z);
                            return;
                        }
                    }
                }
            }
        }
    }

    ChunckComponent CreateChunck(int posX, int posY, int posZ)
    {
        vertices?.Clear();
        triangulos?.Clear();
        uvs?.Clear();

        var chunck = new ChunckComponent();
        chunck.blocks = new byte[size, size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    chunck.blocks[x, y, z] = procedural.GetBlockID(x + posX, y + posY, z + posZ);//GeneratedBlockID(x + posX, y + posY, z + posZ);
                }
            }
        }

        var mesh = GenerateMesh(chunck, posX, posY, posZ);

        var chunckGO = new GameObject($"Chunck {posX} {posY} {posZ}");
        var renderer = chunckGO.AddComponent<MeshRenderer>();
        var meshFilter = chunckGO.AddComponent<MeshFilter>();
        var collider = chunckGO.AddComponent<MeshCollider>();
        renderer.material = mat;
        meshFilter.mesh = mesh;
        collider.sharedMesh = mesh;
        chunckGO.transform.position = new Vector3(posX, posY, posZ);
        chunckGO.transform.SetParent(transform, false);

        chunck.renderer = renderer;
        chunck.meshFilter = meshFilter;
        chunck.collider = collider;
        chunck.pos = chunckGO.transform.position;

        chuncks.Add(new(posX/size, posY/size, posZ/size), chunck);

        return chunck;
    }

    internal ChunckComponent GetChunk(Vector3 globalPosBlock, out Vector3Int chunckKey)
    {
        int xIdx = Mathf.FloorToInt(globalPosBlock.x / size);
        int zIdx = Mathf.FloorToInt(globalPosBlock.z / size);
        int yIdx = Mathf.FloorToInt(globalPosBlock.y / size);

        chunckKey = new Vector3Int(xIdx, yIdx, zIdx);

        if (chuncks.ContainsKey(chunckKey))
        {
            return chuncks[chunckKey];
        }

        return null;
    }

    internal ChunckComponent GetChunk(Vector3 globalPosBlock)
    {
        int xIdx = Mathf.FloorToInt(globalPosBlock.x / size);
        int zIdx = Mathf.FloorToInt(globalPosBlock.z / size);
        int yIdx = Mathf.FloorToInt(globalPosBlock.y / size);

        var key = new Vector3Int(xIdx, yIdx, zIdx);
        if (chuncks.ContainsKey(key))
        {
            return chuncks[key];
        }

        key *= size;
        return CreateChunck(key.x, key.y, key.z);
    }

    Mesh GenerateMesh(ChunckComponent chunck, int posX, int posY, int posZ)
    {
        Mesh mesh = new();
        mesh.Clear();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    if (chunck.blocks[x, y, z] > 0)
                    {
                        BlockUVS b = BlockUVS.GetBlock(chunck.blocks[x, y, z]);

                        //if (x == 0 && z == 0)
                        //    b = new BlockUVS(2, 15);

                        if ((z + 1 >= size && procedural.GetBlockID(x + posX, y + posY, z + 1 + posZ) == 0) || (!(z + 1 >= size) && chunck.blocks[x, y, z + 1] == 0))
                        {
                            CreateBlockSide(BlockSide.Front, x, y, z, b);
                        }
                        if ((z - 1 < 0 && procedural.GetBlockID(x + posX, y + posY, z - 1 + posZ) == 0) || (!(z - 1 < 0) && chunck.blocks[x, y, z - 1] == 0))
                        {
                            CreateBlockSide(BlockSide.Back, x, y, z, b);
                        }
                        if ((x + 1 >= size && procedural.GetBlockID(x + 1 + posX, y + posY, z + posZ) == 0) || (!(x + 1 >= size) && chunck.blocks[x + 1, y, z] == 0))
                        {
                            CreateBlockSide(BlockSide.Right, x, y, z, b);
                        }
                        if ((x - 1 < 0 && procedural.GetBlockID(x - 1 + posX, y + posY, z + posZ) == 0) || (!(x - 1 < 0) && chunck.blocks[x - 1, y, z] == 0))
                        {
                            CreateBlockSide(BlockSide.Left, x, y, z, b);
                        }
                        if ((y + 1 >= size && procedural.GetBlockID(x + posX, y + posY + 1, z + posZ) == 0) || (!(y + 1 >= size) && chunck.blocks[x, y + 1, z] == 0))
                        {
                            CreateBlockSide(BlockSide.Top, x, y, z, b);
                        }
                        if ((y - 1 < 0 && procedural.GetBlockID(x + posX, y + posY - 1, z + posZ) == 0) || (!(y - 1 < 0) && chunck.blocks[x, y - 1, z] == 0))
                        {
                            CreateBlockSide(BlockSide.Bottom, x, y, z, b);
                        }
                        //if (!(y + 1 >= size) && chunck.blocks[x, y + 1, z] == 0 || y + 1 >= size)
                        //{
                        //    CreateBlockSide(BlockSide.Top, x, y, z, b);
                        //}
                        //if (!(y - 1 < 0) && chunck.blocks[x, y - 1, z] == 0)
                        //{
                        //    CreateBlockSide(BlockSide.Bottom, x, y, z, b);
                        //}
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangulos.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.OptimizeReorderVertexBuffer();
        mesh.Optimize();

        return mesh;
    }

    internal Mesh UpdateMesh(ChunckComponent chunck)
    {
        int posX = (int)chunck.renderer.transform.position.x;
        int posY = (int)chunck.renderer.transform.position.y;
        int posZ = (int)chunck.renderer.transform.position.z;

        vertices.Clear();
        triangulos.Clear();
        uvs.Clear();

        Mesh mesh = new();
        mesh.Clear();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    if (chunck.blocks[x, y, z] > 0)
                    {
                        BlockUVS b = BlockUVS.GetBlock(chunck.blocks[x, y, z]); //new(0, 15, 3, 15, 2, 15);

                        var frontCheck = (z + 1 >= size && GetChunk(new Vector3(x + posX, y + posY, z + 1 + posZ)).blocks[x, y, 0] == 0);
                        var backCheck = (z - 1 < 0 && GetChunk(new Vector3(x + posX, y + posY, z - 1 + posZ)).blocks[x, y, size - 1] == 0);
                        var rightCheck = (x + 1 >= size && GetChunk(new Vector3(x + 1 + posX, y + posY, z + posZ)).blocks[0, y, z] == 0);
                        var leftCheck = (x - 1 < 0 && GetChunk(new Vector3(x - 1 + posX, y + posY, z + posZ)).blocks[size - 1, y, z] == 0);
                        var topCheck = (y + 1 >= size && GetChunk(new Vector3(x + posX, y + posY + 1, z + posZ)).blocks[x, 0, z] == 0);
                        var bottomCheck = (y - 1 < 0 && GetChunk(new Vector3(x + posX, y + posY - 1, z + posZ)).blocks[x, size - 1, z] == 0);


                        if ((!(z + 1 >= size) && chunck.blocks[x, y, z + 1] == 0) || frontCheck)
                        {
                            CreateBlockSide(BlockSide.Front, x, y, z, b);
                        }
                        if ((!(z - 1 < 0) && chunck.blocks[x, y, z - 1] == 0) || backCheck)
                        {
                            CreateBlockSide(BlockSide.Back, x, y, z, b);
                        }
                        if ((!(x + 1 >= size) && chunck.blocks[x + 1, y, z] == 0) || rightCheck)
                        {
                            CreateBlockSide(BlockSide.Right, x, y, z, b);
                        }
                        if ((!(x - 1 < 0) && chunck.blocks[x - 1, y, z] == 0) || leftCheck)
                        {
                            CreateBlockSide(BlockSide.Left, x, y, z, b);
                        }
                        if ((!(y + 1 >= size) && chunck.blocks[x, y + 1, z] == 0) || topCheck)
                        {
                            CreateBlockSide(BlockSide.Top, x, y, z, b);
                        }
                        if ((!(y - 1 < 0) && chunck.blocks[x, y - 1, z] == 0) || bottomCheck)
                        {
                            CreateBlockSide(BlockSide.Bottom, x, y, z, b);
                        }
                        //if (!(y + 1 >= size) && chunck.blocks[x, y + 1, z] == 0 || y + 1 >= size)
                        //{
                        //    CreateBlockSide(BlockSide.Top, x, y, z, b);
                        //}
                        //if (!(y - 1 < 0) && chunck.blocks[x, y - 1, z] == 0)
                        //{
                        //    CreateBlockSide(BlockSide.Bottom, x, y, z, b);
                        //}
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangulos.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.OptimizeReorderVertexBuffer();
        mesh.Optimize();

        return mesh;
    }

    void DictionaryInits()
    {
        List<Vector3> verticesFront = new List<Vector3>
            {
                new Vector3( 0, 0, 1 ),
                new Vector3(-1, 0, 1 ),
                new Vector3(-1, 1, 1 ),
                new Vector3( 0, 1, 1 ),
            };
        List<Vector3> verticesBack = new List<Vector3>
            {
                new Vector3( 0, 0, 0 ),
                new Vector3(-1, 0, 0 ),
                new Vector3(-1, 1, 0 ),
                new Vector3( 0, 1, 0 ),
            };
        List<Vector3> verticesRight = new List<Vector3>
            {
                new Vector3( 0, 0, 0 ),
                new Vector3( 0, 0, 1 ),
                new Vector3( 0, 1, 1 ),
                new Vector3( 0, 1, 0 ),
            };
        List<Vector3> verticesLeft = new List<Vector3>
            {
                new Vector3(-1, 0, 0 ),
                new Vector3(-1, 0, 1 ),
                new Vector3(-1, 1, 1 ),
                new Vector3(-1, 1, 0 ),
            };
        List<Vector3> verticesTop = new List<Vector3>
            {
                new Vector3( 0, 1, 0 ),
                new Vector3(-1, 1, 0 ),
                new Vector3(-1, 1, 1 ),
                new Vector3( 0, 1, 1 ),
            };
        List<Vector3> verticesBottom = new List<Vector3>
            {
                new Vector3( 0, 0, 0 ),
                new Vector3(-1, 0, 0 ),
                new Vector3(-1, 0, 1 ),
                new Vector3( 0, 0, 1 ),
            };

        blockVerticesSet.Add(BlockSide.Front, null);
        blockVerticesSet.Add(BlockSide.Back, null);
        blockVerticesSet.Add(BlockSide.Right, null);
        blockVerticesSet.Add(BlockSide.Left, null);
        blockVerticesSet.Add(BlockSide.Top, null);
        blockVerticesSet.Add(BlockSide.Bottom, null);

        blockVerticesSet[BlockSide.Front] = verticesFront;//.ToNativeArray(Allocator.Persistent);
        blockVerticesSet[BlockSide.Back] = verticesBack;//.ToNativeArray(Allocator.Persistent);
        blockVerticesSet[BlockSide.Right] = verticesRight;//.ToNativeArray(Allocator.Persistent);
        blockVerticesSet[BlockSide.Left] = verticesLeft;//.ToNativeArray(Allocator.Persistent);
        blockVerticesSet[BlockSide.Top] = verticesTop;//.ToNativeArray(Allocator.Persistent);
        blockVerticesSet[BlockSide.Bottom] = verticesBottom;
    }

    void InitTriangulos()
    {
        List<int> trianglesFront = new List<int> { 3, 2, 1, 4, 3, 1 };
        List<int> trianglesBack = new List<int> { 1, 2, 3, 1, 3, 4 };
        List<int> trianglesRight = new List<int> { 1, 3, 2, 4, 3, 1 };
        List<int> trianglesLeft = new List<int> { 2, 3, 1, 1, 3, 4 };
        List<int> trianglesTop = new List<int> { 1, 2, 3, 1, 3, 4 };
        List<int> trianglesBottom = new List<int> { 3, 2, 1, 4, 3, 1 };

        blockTrianglesSet.Add(BlockSide.Front, trianglesFront);
        blockTrianglesSet.Add(BlockSide.Back, trianglesBack);
        blockTrianglesSet.Add(BlockSide.Right, trianglesRight);
        blockTrianglesSet.Add(BlockSide.Left, trianglesLeft);
        blockTrianglesSet.Add(BlockSide.Top, trianglesTop);
        blockTrianglesSet.Add(BlockSide.Bottom, trianglesBottom);
    }


    void CreateBlockSide(BlockSide side, int x, int y, int z, BlockUVS b)
    {
        List<Vector3> vrtx = blockVerticesSet[side];
        List<int> trngls = blockTrianglesSet[side];
        int offset = 1;

        triangulos.Add(trngls[0] - offset + vertices.Count);
        triangulos.Add(trngls[1] - offset + vertices.Count);
        triangulos.Add(trngls[2] - offset + vertices.Count);

        triangulos.Add(trngls[3] - offset + vertices.Count);
        triangulos.Add(trngls[4] - offset + vertices.Count);
        triangulos.Add(trngls[5] - offset + vertices.Count);

        vertices.Add(new Vector3(x + vrtx[0].x, y + vrtx[0].y, z + vrtx[0].z)); // 1
        vertices.Add(new Vector3(x + vrtx[1].x, y + vrtx[1].y, z + vrtx[1].z)); // 2
        vertices.Add(new Vector3(x + vrtx[2].x, y + vrtx[2].y, z + vrtx[2].z)); // 3
        vertices.Add(new Vector3(x + vrtx[3].x, y + vrtx[3].y, z + vrtx[3].z)); // 4

        AddUVS(side, b);
    }

    void AddUVS(BlockSide side, BlockUVS b)
    {
        switch (side)
        {
            case BlockSide.Front:
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
                break;
            case BlockSide.Back:
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
                break;
            case BlockSide.Right:
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));

                break;
            case BlockSide.Left:
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
                uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));

                break;
            case BlockSide.Top:
                uvs.Add(new Vector2(TextureOffset * b.TextureX, TextureOffset * b.TextureY));
                uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, TextureOffset * b.TextureY));
                uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, (TextureOffset * b.TextureY) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureX, (TextureOffset * b.TextureY) + TextureOffset));

                break;
            case BlockSide.Bottom:
                uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, TextureOffset * b.TextureYBottom));
                uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, TextureOffset * b.TextureYBottom));
                uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, (TextureOffset * b.TextureYBottom) + TextureOffset));
                uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, (TextureOffset * b.TextureYBottom) + TextureOffset));

                break;

        }

    }

    public byte GeneratedBlockID(int x, int y, int z)
    {
        Random.InitState(888);

        // ============== ��������� ��� =============
        var k = 10000000;// ��� ������ ��� ����

        Vector3 offset = new(Random.value * k, Random.value * k, Random.value * k);

        float noiseX = Mathf.Abs((float)(x + offset.x) / noiseScale / 2);
        float noiseY = Mathf.Abs((float)(y + offset.y) / noiseScale / 2);
        float noiseZ = Mathf.Abs((float)(z + offset.z) / noiseScale / 2);

        float goraValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        goraValue += (30 - y) / 3000f;// World bump
        //goraValue /= y / 1f;// ��� ���� ������;

        byte blockID = 0;
        if (goraValue > 0.35f)
        {
            if (goraValue > 0.3517f)
            {
                blockID = 2;
            }
            else
            {
                blockID = 1;
            }
        }
        // ==========================================

        // =========== �������� �������� ============
        k = 10000;

        offset = new(Random.value * k, Random.value * k, Random.value * k);

        noiseX = Mathf.Abs((float)(x + offset.x) / noiseScale);
        noiseY = Mathf.Abs((float)(y + offset.y) / noiseScale);
        noiseZ = Mathf.Abs((float)(z + offset.z) / noiseScale);

        float noiseValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        noiseValue += (30 - y) / 30f;// World bump
        noiseValue /= y / 8f;

        //cavernas /= y / 19f;
        //cavernas /= 2;
        //Debug.Log($"{noiseValue} --- {y}");

        if (noiseValue > landThresold)
        {
            if (noiseValue > 0.5f)
            {
                blockID = 2;
            }
            else
            {
                blockID = 1;
            }
        }
        // ==========================================

        // =========== �����, ���� ���� =============
        k = 10000;

        offset = new(Random.value * k, Random.value * k, Random.value * k);

        noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale * 2));
        noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale * 3));
        noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale * 2));

        float rockValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        if (rockValue > 0.8f)
        {
            if (rockValue > 0.801f)
                blockID = 2;
            else
                blockID = 1;
        }
        // ==========================================

        // =========== �����, ���� ���� =============
        k = 100;

        offset = new(Random.value * k, Random.value * k, Random.value * k);

        noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 2));
        noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 1));
        noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 2));

        float smallRockValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        if (smallRockValue > smallRockThresold && noiseValue > (landThresold - 0.08f))
        {
            blockID = 2;
            if (smallRockValue < smallRockThresold + 0.01f)
                blockID = 1;
        }
        // ==========================================

        // =========== ������ ========================
        k = 33333;

        offset = new(Random.value * k, Random.value * k, Random.value * k);

        noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        float gravelValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        if (gravelValue > 0.85f && (noiseValue > landThresold))
        {
            blockID = BLOCKS.GRAVEL;
        }
        // ==========================================

        // =========== ����� ========================
        k = 10;

        offset = new(Random.value * k, Random.value * k, Random.value * k);

        noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        float coalValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        if (coalValue > 0.92f && (noiseValue > landThresold))
        {
            blockID = BLOCKS.ORE_COAL;
        }
        // ==========================================

        // =========== �������� ���� ========================
        k = 700;

        offset = new(Random.value * k, Random.value * k, Random.value * k);

        noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        float oreValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        if (oreValue > 0.93f && (noiseValue > landThresold))
        {
            blockID = 30;
        }
        // ==========================================

        // =========== ������� ���� ========================
        k = 635;

        offset = new(Random.value * k, Random.value * k, Random.value * k);

        noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        float saltpeterValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        if (saltpeterValue > 0.935f && (noiseValue > landThresold))
        {
            blockID = BLOCKS.SALTPETER;
        }
        // ==========================================

        // =========== ���� ========================
        k = 364789;

        offset = new(Random.value * k, Random.value * k, Random.value * k);

        noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        float sulfurValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        if (sulfurValue > 0.93f && (noiseValue > landThresold))
        {
            blockID = BLOCKS.ORE_SULFUR;
        }
        // ==========================================


        // ���� ����
        ////////// ��� ��� ////////////////////////////////////////////
        //k = 10000000;// ��� ������ ��� ����

        //offset = new(Random.value * k, Random.value * k, Random.value * k);

        //noiseX = Mathf.Abs((float)(x + offset.x) / noiseScale / 2);
        //noiseY = Mathf.Abs((float)(y + offset.y) / noiseScale / 2);
        //noiseZ = Mathf.Abs((float)(z + offset.z) / noiseScale / 2);

        //float goraValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        //goraValue += (30 - y) / 3000f;// World bump
        //goraValue /= y / 80f;// ��� ���� ������;

        //blockID = 0;
        //if (goraValue > 0.08f && goraValue < 0.3f)
        //{
        //    blockID = 2;
        //}
        ///==============================================



        if (oreValue < minValue)
            minValue = oreValue;
        if (oreValue > maxValue)
            maxValue = oreValue;

        /////////////////////////////////////////////////////////////////////
        //k = 10000000;// ��� ������ ��� ����

        //offset = new(Random.value * k, Random.value * k, Random.value * k);

        //noiseX = Mathf.Abs((float)(x + offset.x) / noiseScale / 2);
        //noiseY = Mathf.Abs((float)(y + offset.y) / noiseScale * 2);
        //noiseZ = Mathf.Abs((float)(z + offset.z) / noiseScale / 2);

        //float goraValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        //goraValue += (30 - y) / 30000f;// World bump
        //goraValue /= y / 8f;

        ////blockID = 0;
        //if (goraValue > 0.1f && goraValue < 0.3f)
        //{
        //    blockID = 2;
        //}
        ////////////////////////////////////////////////////////////////

        return blockID;

        //Random.InitState(888);
        //int k = 1000000;
        //Vector3 offset = new Vector3(Random.value * k, Random.value * k, Random.value * k);

        //Vector3 pos = new Vector3
        //(
        //    x + offset.x,
        //    y + offset.y,
        //    z + offset.z
        //);
        //float noiseX = Mathf.Abs((float)(pos.x + offset.x) / ebota);
        //float noiseY = Mathf.Abs((float)(pos.y + offset.y) / ebota);
        //float noiseZ = Mathf.Abs((float)(pos.z + offset.z) / ebota);
        //#pragma warning disable CS0436 // ��� ����������� � ��������������� �����
        //            var res = noise.snoise(new float3(noiseX, noiseY, noiseZ));//snoise(pos);
        //#pragma warning restore CS0436 // ��� ����������� � ��������������� �����

        //if (y < 3) res = 0.5f;

        //if (res > 0.3f)
        //{
        //    return true;
        //}


    }
    float minValue = float.MaxValue;
    float maxValue = float.MinValue;
}


public enum BlockSide : byte
{
    Front,
    Back,
    Right,
    Left,
    Top,
    Bottom
}

public enum BlockType : byte
{
    Grass,
    Dirt,
    Stone
}
