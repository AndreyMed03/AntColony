using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public float rotationSpeed = 5f;
    public bool isRotationBlocked = false; // Добавлено

    private Vector2 lastTouchPosition;

    void Update()
    {
        if (isRotationBlocked) return; // Блокируем поворот

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDelta = touch.position - lastTouchPosition;
                float mouseX = touchDelta.x * rotationSpeed * Time.deltaTime;
                freeLookCamera.m_XAxis.Value += mouseX;
                lastTouchPosition = touch.position;
            }
        }
        else if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            freeLookCamera.m_XAxis.Value += mouseX * rotationSpeed * Time.deltaTime;
        }
    }
}
