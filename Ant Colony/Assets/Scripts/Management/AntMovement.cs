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
    private float lastTapTime = 0f;
    private float doubleTapMaxDelay = 0.3f;

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

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            MoveAgentToClick(Input.mousePosition);
        }
#else
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                float timeSinceLastTap = Time.time - lastTapTime;

                if (timeSinceLastTap <= doubleTapMaxDelay)
                {
                    MoveAgentToClick(touch.position);
                    lastTapTime = 0f; // сброс
                }
                else
                {
                    lastTapTime = Time.time;
                }
            }
        }
#endif
    }

    private void MoveAgentToClick(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ignoreLayerMask))
        {
            agent.SetDestination(hit.point);
        }
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
