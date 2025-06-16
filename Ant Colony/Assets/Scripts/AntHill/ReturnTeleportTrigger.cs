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

    public static ReturnTeleportTrigger ActiveInstance;

    public void Initialize(Vector3 position,
                           CameraZoomController zoomController,
                           CameraZoneCleaning zoneCleaning,
                           DiggingManager digging)
    {
        returnPosition = position;
        cameraZoomController = zoomController;
        cameraZoneCleaning = zoneCleaning;
        diggingManager = digging;

        ActiveInstance = this;
    }

    private void OnEnable()
    {
        //Debug.Log("ReturnTeleportTrigger: OnEnable() вызван");
        //Debug.Log("OnEnable вызван у " + gameObject.name + " (" + GetInstanceID() + ")");

        if (returnButton == null)
        {
            Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (Button btn in allButtons)
            {
                if (btn.name == "Climb_Up_Button")
                {
                    returnButton = btn;
                    returnButton.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ant"))
        {
            player = other.gameObject;
            if (returnButton != null)
                returnButton.gameObject.SetActive(true);
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
        //Debug.Log("OnReturnButtonClicked вызван у " + gameObject.name + " (" + GetInstanceID() + ")");

        if (player == null)
            player = GameObject.FindWithTag("Ant");
        if (player == null) return;

        Vector2 offset2D = Random.insideUnitCircle.normalized * 15f;
        Vector3 surfaceReturnPos = returnPosition + new Vector3(offset2D.x, 10f, offset2D.y);

        if (Physics.Raycast(surfaceReturnPos, Vector3.down, out RaycastHit hit, 50f))
        {
            surfaceReturnPos.y = hit.point.y + 0.05f;
        }
        else
        {
            surfaceReturnPos.y = returnPosition.y;
        }

        var agent = player.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.Warp(surfaceReturnPos);
        else
            player.transform.position = surfaceReturnPos;

        if (cameraZoomController != null)
            cameraZoomController.ResetZoom();
        if (cameraZoneCleaning != null)
            cameraZoneCleaning.enabled = false;
        if (diggingManager != null)
            diggingManager.enabled = false;

        if (returnButton != null)
            returnButton.gameObject.SetActive(false);

        foreach (Button btn in Resources.FindObjectsOfTypeAll<Button>())
        {
            if (btn.name == "Digging_Button")
                btn.gameObject.SetActive(false);
            if (btn.name == "Go_To_AntHill_Button")
                btn.gameObject.SetActive(true);
        }

        var teleportTrigger = FindObjectOfType<TeleportTrigger>();
        if (teleportTrigger != null)
            teleportTrigger.AddAntToCooldown(player, 1.5f);
    }
}

