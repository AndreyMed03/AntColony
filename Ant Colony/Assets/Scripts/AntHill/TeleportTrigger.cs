using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TeleportTrigger : MonoBehaviour
{
    private GameObject targetSmallChunk;
    private Button returnButton;

    public GameObject lowerTriggerPrefab; // Префаб нижней зоны триггера
    public void Initialize(GameObject smallChunk)
    {
        targetSmallChunk = smallChunk;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ant"))
        {
            StartCoroutine(DigDownAndTeleport(other.gameObject));
        }
    }

    private IEnumerator DigDownAndTeleport(GameObject ant)
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

        // Ищем нижний чанк по тегу
        GameObject lowerChunkObj = GameObject.FindGameObjectWithTag("LowerChunk");
        if (lowerChunkObj != null)
        {
            // Позиция под входом, но на уровне нижнего чанка
            Vector3 targetPos = new Vector3(
                transform.position.x,
                lowerChunkObj.transform.position.y + 0.5f,
                transform.position.z
            );

            // Телепортируем муравья
            var agent = ant.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.Warp(targetPos);
            else
                ant.transform.position = targetPos;

            Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (Button btn in allButtons)
            {
                if (btn.name == "Climb_Up_Button")
                {
                    returnButton = btn;
                    returnButton.gameObject.SetActive(true);
                }
            }

            Debug.Log("Ant teleported to lower chunk: " + targetPos);

            // Создаём нижнюю триггер-зону, если её ещё нет
            if (lowerTriggerPrefab != null)
            {
                Vector3 surfacePos = transform.position;
                if (!TriggerAlreadyExistsAt(surfacePos))
                {
                    GameObject lowerTrigger = Instantiate(lowerTriggerPrefab, surfacePos, Quaternion.identity);

                    var returnTrigger = lowerTrigger.GetComponent<ReturnTeleportTrigger>();
                    if (returnTrigger != null)
                    {
                        returnTrigger.SetReturnPoint(surfacePos);
                    }

                    Debug.Log("Created new ReturnTeleportTrigger at: " + surfacePos);
                }
                else
                {
                    Debug.Log("ReturnTeleportTrigger already exists at: " + surfacePos);
                }
            }
        }
        else
        {
            Debug.LogWarning("LowerChunk не найден!");
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
