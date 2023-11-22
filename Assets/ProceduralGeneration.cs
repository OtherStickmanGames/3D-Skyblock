using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    public float yCorrect = 1f;
    public int noiseScale = 100;
    public float landThresold = 0.11f;
    float smallRockThresold = 0.8f;

    float minValue = float.MaxValue;
    float maxValue = float.MinValue;

    public byte GetBlockID(int x, int y, int z)
    {
        Random.InitState(888);

        // ============== ��������� ��� =============
        var k = 1000;//10000000;// ��� ������ ��� ����

        Vector3 offset = new(Random.value * k, Random.value * k, Random.value * k);

        float noiseX = Mathf.Abs((float)(x + offset.x) / noiseScale / 2);
        float noiseY = Mathf.Abs((float)(y + offset.y) / noiseScale / 2);
        float noiseZ = Mathf.Abs((float)(z + offset.z) / noiseScale / 2);

        //float goraValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        ////goraValue += (30 - y) / 3000f;// World bump
        ////goraValue /= y / 1f;// ��� ���� ������;

        byte blockID = 0;
        //if (goraValue > 0.35f)
        //{
        //    if (goraValue > 0.3517f)
        //    {
        //        blockID = 2;
        //    }
        //    else
        //    {
        //        blockID = 1;
        //    }
        //}
        // ==========================================

        // =========== �������� �������� ============
        k = 10000;

        offset = new(Random.value * k, Random.value * k, Random.value * k);

        noiseX = Mathf.Abs((float)(x + offset.x) / noiseScale);
        noiseY = Mathf.Abs((float)(y + offset.y) / noiseScale);
        noiseZ = Mathf.Abs((float)(z + offset.z) / noiseScale);

        float noiseValue = SimplexNoise.Noise.Generate(noiseX, noiseY / yCorrect, noiseZ);

        //noiseValue += (30 - y) / 30f;// World bump
        //noiseValue /= y / 8f;

        //cavernas /= y / 19f;
        //cavernas /= 2;
        //Debug.Log($"{noiseValue} --- {y}");

        if (noiseValue > landThresold)
        {
            if (noiseValue > 0.5f)
            {
                blockID = 1;
            }
            else
            {
                blockID = 2;
            }
        }
        // ==========================================

        //// =========== �����, ���� ���� =============
        //k = 10000;

        //offset = new(Random.value * k, Random.value * k, Random.value * k);

        //noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale * 2));
        //noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale * 3));
        //noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale * 2));

        //float rockValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        //if (rockValue > 0.8f)
        //{
        //    if (rockValue > 0.801f)
        //        blockID = 2;
        //    else
        //        blockID = 1;
        //}
        //// ==========================================

        //// =========== �����, ���� ���� =============
        //k = 100;

        //offset = new(Random.value * k, Random.value * k, Random.value * k);

        //noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 2));
        //noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 1));
        //noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 2));

        //float smallRockValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        //if (smallRockValue > smallRockThresold && noiseValue > (landThresold - 0.08f))
        //{
        //    blockID = 2;
        //    if (smallRockValue < smallRockThresold + 0.01f)
        //        blockID = 1;
        //}
        //// ==========================================

        //// =========== ������ ========================
        //k = 33333;

        //offset = new(Random.value * k, Random.value * k, Random.value * k);

        //noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        //noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        //noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        //float gravelValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        //if (gravelValue > 0.85f && (noiseValue > landThresold))
        //{
        //    blockID = BLOCKS.GRAVEL;
        //}
        //// ==========================================

        //// =========== ����� ========================
        //k = 10;

        //offset = new(Random.value * k, Random.value * k, Random.value * k);

        //noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        //noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        //noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        //float coalValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        //if (coalValue > 0.92f && (noiseValue > landThresold))
        //{
        //    blockID = BLOCKS.ORE_COAL;
        //}
        //// ==========================================

        //// =========== �������� ���� ========================
        //k = 700;

        //offset = new(Random.value * k, Random.value * k, Random.value * k);

        //noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        //noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        //noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        //float oreValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        //if (oreValue > 0.93f && (noiseValue > landThresold))
        //{
        //    blockID = 30;
        //}
        //// ==========================================

        //// =========== ������� ���� ========================
        //k = 635;

        //offset = new(Random.value * k, Random.value * k, Random.value * k);

        //noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        //noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        //noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        //float saltpeterValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        //if (saltpeterValue > 0.935f && (noiseValue > landThresold))
        //{
        //    blockID = BLOCKS.SALTPETER;
        //}
        //// ==========================================

        //// =========== ���� ========================
        //k = 364789;

        //offset = new(Random.value * k, Random.value * k, Random.value * k);

        //noiseX = Mathf.Abs((float)(x + offset.x) / (noiseScale / 9));
        //noiseY = Mathf.Abs((float)(y + offset.y) / (noiseScale / 9));
        //noiseZ = Mathf.Abs((float)(z + offset.z) / (noiseScale / 9));

        //float sulfurValue = SimplexNoise.Noise.Generate(noiseX, noiseY, noiseZ);

        //if (sulfurValue > 0.93f && (noiseValue > landThresold))
        //{
        //    blockID = BLOCKS.ORE_SULFUR;
        //}
        //// ==========================================


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



        //if (oreValue < minValue)
        //    minValue = oreValue;
        //if (oreValue > maxValue)
        //    maxValue = oreValue;

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
}
