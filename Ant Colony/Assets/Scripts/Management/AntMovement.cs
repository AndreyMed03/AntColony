using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class AntMovement : MonoBehaviour
{
    private Camera mainCamera;
    private NavMeshAgent agent;

    private bool isPlacementMode = false;
    private int ignoreLayerMask;

    void Start()
    {
        mainCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();

        // Игнорируем слой Transparent Piece
        int transparentLayer = LayerMask.NameToLayer("Transparent Piece");
        ignoreLayerMask = ~(1 << transparentLayer);

        PlaceAntOnGround();
    }

    void Update()
    {
        if (isPlacementMode) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ignoreLayerMask))
            {
                agent.SetDestination(hit.point);
            }
        }
#if !UNITY_EDITOR
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ignoreLayerMask))
            {
                agent.SetDestination(hit.point);
            }
        }
#endif
    }

    public void SetPlacementMode(bool state)
    {
        isPlacementMode = state;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        agent.SetDestination(targetPosition);
    }

    private void PlaceAntOnGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity, ignoreLayerMask))
        {
            transform.position = hit.point;
            agent.Warp(hit.point);
        }
        else
        {
            Debug.LogWarning("Не удалось найти поверхность под муравьём. Проверьте, что карта сгенерирована.");
        }
    }
}
