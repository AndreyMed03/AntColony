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
    private float delayBetweenSpawns = 0.1f;

    [SerializeField]
    private string navMeshObjectName = "NavMesh"; // имя объекта с нужным NavMeshSurface

    private NavMeshSurface navMeshSurface;

    private void Start()
    {
        // Находим объект по имени и берём у него NavMeshSurface
        GameObject navMeshObj = GameObject.Find(navMeshObjectName);
        if (navMeshObj != null)
        {
            navMeshSurface = navMeshObj.GetComponent<NavMeshSurface>();
        }
        else
        {
            Debug.LogWarning($"Не найден объект с именем {navMeshObjectName}.");
        }

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
        else
        {
            Debug.LogWarning("NavMeshSurface для ландшафта не найден.");
        }
    }
}
