using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunckComponent
{
    public MeshRenderer renderer;
    public MeshFilter meshFilter;
    public MeshCollider collider;
    public Vector3 pos;

    public byte[,,] blocks;
}
