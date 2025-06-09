using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;

    private float originalTopHeight;
    private float originalMiddleHeight;
    private float originalBottomHeight;
    private float originalTopRadius;
    private float originalMiddleRadius;
    private float originalBottomRadius;

    private void Awake()
    {
        if (freeLookCamera == null)
            freeLookCamera = GetComponent<CinemachineFreeLook>();

        // Сохраняем начальные параметры камеры
        originalTopHeight = freeLookCamera.m_Orbits[0].m_Height;
        originalMiddleHeight = freeLookCamera.m_Orbits[1].m_Height;
        originalBottomHeight = freeLookCamera.m_Orbits[2].m_Height;
        originalTopRadius = freeLookCamera.m_Orbits[0].m_Radius;
        originalMiddleRadius = freeLookCamera.m_Orbits[1].m_Radius;
        originalBottomRadius = freeLookCamera.m_Orbits[2].m_Radius;
    }

    public void ZoomIn()
    {
        freeLookCamera.m_Orbits[0].m_Height = 65f;
        freeLookCamera.m_Orbits[1].m_Height = 65f;
        freeLookCamera.m_Orbits[2].m_Height = 65f;
        freeLookCamera.m_Orbits[0].m_Radius = 70f;
        freeLookCamera.m_Orbits[1].m_Radius = 70f;
        freeLookCamera.m_Orbits[2].m_Radius = 70f;
    }

    public void ResetZoom()
    {
        freeLookCamera.m_Orbits[0].m_Height = originalTopHeight;
        freeLookCamera.m_Orbits[1].m_Height = originalMiddleHeight;
        freeLookCamera.m_Orbits[2].m_Height = originalBottomHeight;
        freeLookCamera.m_Orbits[0].m_Radius = originalTopRadius;
        freeLookCamera.m_Orbits[1].m_Radius = originalMiddleRadius;
        freeLookCamera.m_Orbits[2].m_Radius = originalBottomRadius;
    }
}

