using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TeleportTrigger : MonoBehaviour
{
    private GameObject targetSmallChunk;
    private Button returnButton;

    public NavMeshBaker navMeshBaker;
    public GameObject lowerTriggerPrefab;
    public CameraZoomController cameraZoomController;
    public CameraZoneCleaning cameraZoneCleaning;
    public DiggingManager diggingManager;

    private HashSet<GameObject> antCooldownSet = new HashSet<GameObject>();

    public void AddAntToCooldown(GameObject ant, float duration)
    {
        if (!antCooldownSet.Contains(ant))
            StartCoroutine(RemoveCooldownAfterDelay(ant, duration));
    }

    private IEnumerator RemoveCooldownAfterDelay(GameObject ant, float delay)
    {
        antCooldownSet.Add(ant);
        yield return new WaitForSeconds(delay);
        antCooldownSet.Remove(ant);
    }

    private void Start()
    {
        navMeshBaker = FindObjectOfType<NavMeshBaker>();
        cameraZoomController = FindObjectOfType<CameraZoomController>();
        cameraZoneCleaning = FindObjectOfType<CameraZoneCleaning>();
        if (cameraZoneCleaning != null) cameraZoneCleaning.enabled = false;
        diggingManager = FindObjectOfType<DiggingManager>();
        if (diggingManager != null) diggingManager.enabled = false;
    }

    public void Initialize(GameObject smallChunk)
    {
        targetSmallChunk = smallChunk;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ant") && !antCooldownSet.Contains(other.gameObject))
        {
            StartCoroutine(HandleTeleport(other.gameObject));
        }
    }


    private IEnumerator HandleTeleport(GameObject ant)
    {
        Vector3 targetPos;
        GameObject lowerChunkObj = GameObject.FindGameObjectWithTag("LowerChunk");

        if (lowerChunkObj == null)
        {
            Debug.LogWarning("LowerChunk не найден!");
            yield break;
        }

        targetPos = new Vector3(
            transform.position.x,
            lowerChunkObj.transform.position.y + 0.5f,
            transform.position.z
        );

        if (TriggerAlreadyExistsAt(targetPos))
        {
            SimpleTeleport(ant, targetPos);
        }
        else
        {
            yield return StartCoroutine(DigDownAndCreateAccess(ant, targetPos));
        }
    }

    private IEnumerator DigDownAndCreateAccess(GameObject ant, Vector3 targetPos)
    {
        if (targetSmallChunk == null)
        {
            Debug.LogWarning("Target small chunk is not set.");
            yield break;
        }

        List<Transform> piecesToDestroy = new List<Transform>();
        Vector3 triggerPos = transform.position;
        float triggerRadiusXZ = 30f;

        foreach (Transform piece in targetSmallChunk.transform)
        {
            if (piece.CompareTag("LowerChunk")) continue;

            Vector3 piecePos = piece.position;
            float distXZ = Vector2.Distance(new Vector2(triggerPos.x, triggerPos.z), new Vector2(piecePos.x, piecePos.z));

            if (distXZ < triggerRadiusXZ && piecePos.y < triggerPos.y)
            {
                piecesToDestroy.Add(piece);
            }
        }

        piecesToDestroy.Sort((a, b) => b.position.y.CompareTo(a.position.y));

        foreach (Transform piece in piecesToDestroy)
        {
            Destroy(piece.gameObject);
            yield return new WaitForSeconds(0.01f);
        }
        if (navMeshBaker != null)
        {
            navMeshBaker.Bake();
        }

        // Телепортация
        SimpleTeleport(ant, targetPos);

        // Создаём нижнюю триггер-зону
        if (lowerTriggerPrefab != null)
        {
            Vector3 surfacePos = transform.position;
            GameObject lowerTrigger = Instantiate(lowerTriggerPrefab, targetPos, Quaternion.identity);

            var returnTrigger = lowerTrigger.GetComponent<ReturnTeleportTrigger>();
            if (returnTrigger != null)
            {
                returnTrigger.SetReturnPoint(surfacePos);
            }
        }
    }

    private void SimpleTeleport(GameObject ant, Vector3 targetPos)
    {
        var agent = ant.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.Warp(targetPos);
        else
            ant.transform.position = targetPos;

        if (cameraZoomController != null)
            cameraZoomController.ZoomIn();
        if (cameraZoneCleaning != null)
            cameraZoneCleaning.enabled = true;
        if (diggingManager != null)
            diggingManager.enabled = true;

        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button btn in allButtons)
        {
            if (btn.name == "Climb_Up_Button")
            {
                returnButton = btn;
                returnButton.gameObject.SetActive(true);
            }
            else if (btn.name == "Digging_Button")
            {
                btn.gameObject.SetActive(true);
            }
        }
    }

    private bool TriggerAlreadyExistsAt(Vector3 position, float threshold = 1f)
    {
        var triggers = FindObjectsOfType<ReturnTeleportTrigger>();
        foreach (var trigger in triggers)
        {
            if (Vector3.Distance(trigger.transform.position, position) < threshold)
                return true;
        }
        return false;
    }
}
