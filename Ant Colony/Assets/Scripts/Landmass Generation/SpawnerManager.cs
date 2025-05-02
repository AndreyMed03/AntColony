using KrisDevelopment.EnvSpawn;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField]
    private EnviroSpawn_CS[] spawners;

    [SerializeField]
    private float delayBetweenSpawns = 0.5f;

    private NavMeshSurface navMeshSurface;

    private void Start()
    {
        navMeshSurface = FindObjectOfType<NavMeshSurface>();
        GenerateAllSequentially();
    }

    public void GenerateAllSequentially()
    {
        StartCoroutine(GenerateSequentially());
    }

    private IEnumerator GenerateSequentially()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.InstantiateNew();
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }
}
