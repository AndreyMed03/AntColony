using KrisDevelopment.EnvSpawn;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField]
    private EnviroSpawn_CS[] spawners;  // Массив всех спавнеров

    [SerializeField]
    private float delayBetweenSpawns = 0.5f;  // Задержка между каждым спавнером

    private NavMeshSurface navMeshSurface;  // Для ссылки на NavMeshSurface

    private void Start()
    {
        navMeshSurface = FindObjectOfType<NavMeshSurface>();  // Найти NavMeshSurface в сцене
        GenerateAllSequentially();
    }

    public void GenerateAllSequentially()
    {
        StartCoroutine(GenerateSequentially());
    }

    private IEnumerator GenerateSequentially()
    {
        yield return new WaitForSeconds(0.5f);  // Задержка перед началом спавна

        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.InstantiateNew();  // Генерация объектов спавнером
                yield return new WaitForSeconds(delayBetweenSpawns);  // Задержка между спавнами
            }
        }

        // После завершения всех спавнов перестроить NavMesh
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }
}
