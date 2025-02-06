using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutoTarget : MonoBehaviour
{

    [SerializeField] private string targetTag = "Ant"; // Тег объекта, который камера будет искать
    [SerializeField] private float searchInterval = 0.5f; // Интервал поиска объекта (в секундах)

    private CinemachineFreeLook freeLookCamera; // Ссылка на CineMachine Free Look камеру
    private Transform currentTarget; // Текущая цель

    private void Awake()
    {
        // Автоматически находим компонент CineMachine Free Look
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        if (freeLookCamera == null)
        {
            Debug.LogError("На объекте отсутствует компонент CinemachineFreeLook. Пожалуйста, добавьте его.");
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(FindTarget), 0f, searchInterval);
    }

    private void FindTarget()
    {
        if (currentTarget != null) return;

        GameObject targetObject = GameObject.FindWithTag(targetTag);

        if (targetObject != null)
        {
            currentTarget = targetObject.transform;
            SetCameraTarget(currentTarget);
        }
    }


    private void SetCameraTarget(Transform target)
    {
        if (freeLookCamera != null && target != null)
        {
            freeLookCamera.Follow = target;
            freeLookCamera.LookAt = target;
        }
    }
}
