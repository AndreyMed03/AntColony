using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntHillChunkManager : MonoBehaviour
{
    public List<GameObject> allLargeChunks;

    public string smallChunkTag = "SmallChunk";
    public string lowerChunkTag = "LowerChunk";

    public void HandleAnthillPlacement(GameObject anthill)
    {
        Vector3 position = anthill.transform.position;

        GameObject closestChunk = null;
        float minDistance = float.MaxValue;

        foreach (GameObject chunk in allLargeChunks)
        {
            float distance = Vector3.Distance(chunk.transform.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestChunk = chunk;
            }
        }

        if (closestChunk == null)
        {
            Debug.LogWarning("Не найден ближайший LargeChunk!");
            return;
        }

        closestChunk.SetActive(true);

        GameObject closestSmallChunk = FindClosestSmallChunk(closestChunk, position);
        if (closestSmallChunk != null)
        {
            ActivateSmallChunk(closestSmallChunk);

            Transform teleportZone = anthill.transform.Find("TeleportZone");
            if (teleportZone != null)
            {
                var trigger = teleportZone.GetComponent<TeleportTrigger>();
                if (trigger != null)
                {
                    trigger.Initialize(closestSmallChunk);
                }
                else
                {
                    Debug.LogWarning("TeleportZoneTrigger не найден на TeleportZone!");
                }
            }
            else
            {
                Debug.LogWarning("TeleportZone не найден у anthill!");
            }
        }
    }


    private GameObject FindClosestSmallChunk(GameObject parent, Vector3 position)
    {
        GameObject closest = null;
        float minDistance = float.MaxValue;

        foreach (Transform child in parent.transform)
        {
            if (!child.CompareTag(smallChunkTag)) continue;

            float distance = Vector3.Distance(child.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = child.gameObject;
            }
        }

        return closest;
    }

    private void ActivateSmallChunk(GameObject chunk)
    {
        foreach (Transform piece in chunk.transform)
        {
            if (piece.GetComponent<MeshCollider>() == null)
                piece.gameObject.AddComponent<MeshCollider>();
        }

        chunk.SetActive(true);
    }
}
