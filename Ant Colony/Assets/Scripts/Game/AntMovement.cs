using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AntMovement : MonoBehaviour
{
    private Camera mainCamera;
    private NavMeshAgent agent;

    void Start()
    {
        mainCamera = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        PlaceAntOnGround();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            RaycastHit hit;

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    agent.SetDestination(hit.point);
                }
            }
            else if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (Physics.Raycast(mainCamera.ScreenPointToRay(touch.position), out hit))
                {
                    agent.SetDestination(hit.point);
                }
            }
        }
    }

    private void PlaceAntOnGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, Mathf.Infinity))
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
