using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	BlockItemSpawner blockItemSpawner;
	List<IUpdateble> updatables = new();


    private void Start()
    {
		updatables.Add(new BlockItemSpawner());
    }

    private void Update()
    {
        foreach (var item in updatables) item.Update();
		
    }

    public static readonly Vector3[] faceChecks = new Vector3[6]
	{
		new Vector3( 0.0f, 0.0f,-1.0f),
		new Vector3( 0.0f, 0.0f, 1.0f),
		new Vector3( 0.0f, 1.0f, 0.0f),
		new Vector3( 0.0f,-1.0f, 0.0f),
		new Vector3(-1.0f, 0.0f, 0.0f),
		new Vector3( 1.0f, 0.0f, 0.0f),
	};
}
