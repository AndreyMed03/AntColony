using KrisDevelopment.EnvSpawn;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField]
    private EnviroSpawn_CS[] spawners;  // Массив всех спавнеров

    [SerializeField]
    private float delayBetweenSpawns = 0.5f;  // Задержка между каждым спавнером

    private void Start()
    {
        GenerateAllSequentially();
    }

    //private IEnumerator GenerateWithInitialDelay()
    //{
    //    yield return new WaitForSeconds(2.0f);  // Задержка перед первым спавнером

    //    GenerateAllSequentially();  // Запуск генерации всех спавнеров
    //}
    public void GenerateAllSequentially()
    {
        StartCoroutine(GenerateSequentially());
    }

    private IEnumerator GenerateSequentially()
    {
        yield return new WaitForSeconds(2.0f);
        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.InstantiateNew(); 
                
                // Генерация всех объектов для текущего спавнера
                
                yield return new WaitForSeconds(delayBetweenSpawns);  // Задержка между генерацией каждого спавнера
            }
        }
    }
}
