using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutoTarget : MonoBehaviour
{

    [SerializeField] private string targetTag = "Ant"; // Тег объекта, который камера будет искать
    [SerializeField] private float searchInterval = 0.5f;

    private CinemachineFreeLook freeLookCamera;
    private Transform currentTarget;

    private void Awake()
    {
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
