using KrisDevelopment.EnvSpawn;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField]
    private EnviroSpawn_CS[] spawners;  // ������ ���� ���������

    [SerializeField]
    private float delayBetweenSpawns = 0.5f;  // �������� ����� ������ ���������

    private void Start()
    {
        GenerateAllSequentially();
    }

    //private IEnumerator GenerateWithInitialDelay()
    //{
    //    yield return new WaitForSeconds(2.0f);  // �������� ����� ������ ���������

    //    GenerateAllSequentially();  // ������ ��������� ���� ���������
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
                
                // ��������� ���� �������� ��� �������� ��������
                
                yield return new WaitForSeconds(delayBetweenSpawns);  // �������� ����� ���������� ������� ��������
            }
        }
    }
}
