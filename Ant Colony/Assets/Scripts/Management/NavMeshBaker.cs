using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    [SerializeField]
    private string groundObjectName = "Ground";

    private NavMeshSurface groundNavMeshSurface;

    private void Awake()
    {
        GameObject groundObj = GameObject.Find(groundObjectName);
        if (groundObj != null)
            groundNavMeshSurface = groundObj.GetComponent<NavMeshSurface>();

        if (groundNavMeshSurface == null)
            Debug.LogWarning("NavMeshSurface на объекте Ground не найден.");
    }

    public void Bake()
    {
        if (groundNavMeshSurface != null)
            groundNavMeshSurface.BuildNavMesh();
        else
            Debug.LogWarning("Не удалось запечь NavMesh: компонент отсутствует.");
    }
}
