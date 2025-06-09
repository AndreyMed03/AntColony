using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ReturnTeleportTrigger : MonoBehaviour
{
    private Button returnButton;
    private Vector3 returnPosition;
    private GameObject player;

    public CameraZoomController cameraZoomController;
    public CameraZoneCleaning cameraZoneCleaning;
    public DiggingManager diggingManager;

    public void SetReturnPoint(Vector3 position)
    {
        returnPosition = position;
    }

    private void Start()
    {
        cameraZoomController = FindObjectOfType<CameraZoomController>();
        cameraZoneCleaning = FindObjectOfType<CameraZoneCleaning>();
        diggingManager = FindObjectOfType<DiggingManager>();

        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button btn in allButtons)
        {
            if (btn.name == "Climb_Up_Button")
            {
                returnButton = btn;
                returnButton.onClick.AddListener(OnReturnButtonClicked);
                returnButton.gameObject.SetActive(false);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ant"))
        {
            player = other.gameObject;
            if (returnButton != null)
                returnButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ant"))
        {
            if (player == other.gameObject)
                player = null;

            if (returnButton != null)
                returnButton.gameObject.SetActive(false);
        }
    }

    public void OnReturnButtonClicked()
    {
        if (player == null) return;

        // Смещение на поверхности — чтобы не появиться точно в центре входа
        Vector2 offset2D = Random.insideUnitCircle.normalized * 15f;
        Vector3 surfaceReturnPos = returnPosition + new Vector3(offset2D.x, 10f, offset2D.y); // 10f по Y выше для raycast

        // Raycast вниз, чтобы точно встать на землю
        if (Physics.Raycast(surfaceReturnPos, Vector3.down, out RaycastHit hit, 50f))
        {
            surfaceReturnPos.y = hit.point.y + 0.05f; // немного над землёй
        }
        else
        {
            surfaceReturnPos.y = returnPosition.y; // fallback, не прибавляем 0.5!
        }

        var agent = player.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.Warp(surfaceReturnPos);
        else
            player.transform.position = surfaceReturnPos;

        if (cameraZoomController != null)
            cameraZoomController.ResetZoom(); // Возврат масштаба
        if (cameraZoneCleaning != null)
            cameraZoneCleaning.enabled = false; // Отключение чистки зоны камеры
        if (diggingManager != null)
            diggingManager.enabled = false; // Отключение механизма копания

        if (returnButton != null)
            returnButton.gameObject.SetActive(false);
        foreach (Button btn in Resources.FindObjectsOfTypeAll<Button>())
        {
            if (btn.name == "Digging_Button")
                btn.gameObject.SetActive(false);
        }

        var teleportTrigger = FindObjectOfType<TeleportTrigger>();
        if (teleportTrigger != null)
        {
            teleportTrigger.AddAntToCooldown(player, 1.5f); // 1.5 секунды "иммунитет"
        }
    }
}
