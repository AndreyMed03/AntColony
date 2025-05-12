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

    public void SetReturnPoint(Vector3 position)
    {
        returnPosition = position;
    }

    private void Start()
    {
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

        Debug.LogWarning("ReturnButton not found in scene.");
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

        // —мещение на поверхности Ч чтобы не по€витьс€ точно в центре входа
        Vector2 offset2D = Random.insideUnitCircle.normalized * 5f;
        Vector3 surfaceReturnPos = returnPosition + new Vector3(offset2D.x, 10f, offset2D.y); // 10f по Y выше дл€ raycast

        // Raycast вниз, чтобы точно встать на землю
        if (Physics.Raycast(surfaceReturnPos, Vector3.down, out RaycastHit hit, 50f))
        {
            surfaceReturnPos.y = hit.point.y + 0.05f; // немного над землЄй
        }
        else
        {
            Debug.LogWarning("Raycast вниз не нашЄл землю Ч используем запасную высоту.");
            surfaceReturnPos.y = returnPosition.y; // fallback, не прибавл€ем 0.5!
        }

        var agent = player.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.Warp(surfaceReturnPos);
        else
            player.transform.position = surfaceReturnPos;

        Debug.Log("Ant returned to surface at: " + surfaceReturnPos);

        if (returnButton != null)
            returnButton.gameObject.SetActive(false);
    }
}
