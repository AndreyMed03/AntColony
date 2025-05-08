using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class AntHillPlacement : MonoBehaviour
{
    public GameObject anthillPrefab;
    public GameObject applyButton;
    public GameObject cancelButton;
    public GameObject spawnButton;
    public GameObject goToAntHillButton;

    private GameObject currentGhost;
    private GameObject lastPlacedAnthill;

    private Camera mainCamera;
    private bool isPlacing = false;
    private bool isDragging = false;
    private bool hasRotated = false;

    private AntMovement antMovement;

    void Start()
    {
        mainCamera = Camera.main;
        applyButton.SetActive(false);
        cancelButton.SetActive(false);
        goToAntHillButton.SetActive(false);

        antMovement = FindObjectOfType<AntMovement>();
    }

    public void StartPlacement()
    {
        if (antMovement == null) return;
        if (isPlacing) return;

        isPlacing = true;
        antMovement.SetPlacementMode(true);

        currentGhost = Instantiate(anthillPrefab);
        SetTransparent(currentGhost, 0.5f);

        applyButton.SetActive(true);
        cancelButton.SetActive(true);
    }

    void Update()
    {
        if (antMovement == null) antMovement = FindObjectOfType<AntMovement>();
        if (!isPlacing || currentGhost == null) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            isDragging = true;
            if (!hasRotated)
            {
                float randomY = Random.Range(0f, 360f);
                currentGhost.transform.rotation = Quaternion.Euler(0f, randomY, 0f);
                hasRotated = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            hasRotated = false;
        }
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (isDragging)
            MoveGhost(Input.mousePosition);
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) 
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;
                isDragging = true;
                if (!hasRotated)
                {
                    float randomY = Random.Range(0f, 360f);
                    currentGhost.transform.rotation = Quaternion.Euler(0f, randomY, 0f);
                    hasRotated = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) 
            {
                isDragging = false;
                hasRotated = false;
            }

            if (isDragging)
                MoveGhost(touch.position);
        }
#endif
    }

    private void MoveGhost(Vector2 screenPosition)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentGhost.transform.position = hit.point;
        }
    }

    public void ConfirmPlacement()
    {
        if (currentGhost == null) return;

        SetTransparent(currentGhost, 1f);
        AddPhysics(currentGhost);

        lastPlacedAnthill = currentGhost;
        spawnButton.SetActive(false);
        goToAntHillButton.SetActive(true);

        ResetPlacementState();
    }

    public void CancelPlacement()
    {
        if (currentGhost != null)
            Destroy(currentGhost);

        ResetPlacementState();
    }

    private void ResetPlacementState()
    {
        isPlacing = false;
        isDragging = false;
        applyButton.SetActive(false);
        cancelButton.SetActive(false);
        antMovement.SetPlacementMode(false);
        hasRotated = false;
    }

    private void SetTransparent(GameObject obj, float alpha)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in renderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;

                mat.SetFloat("_Mode", alpha < 1f ? 2f : 0f);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", alpha < 1f ? 0 : 1);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = alpha < 1f ? 3000 : -1;
            }
        }
    }

    private void AddPhysics(GameObject obj)
    {
        if (obj.GetComponent<Collider>() == null)
            obj.AddComponent<MeshCollider>();

        // Добавляем коллайдер к TeleportZone
        Transform teleportZone = obj.transform.Find("TeleportZone");
        if (teleportZone != null)
        {
            if (teleportZone.GetComponent<Collider>() == null)
            {
                CapsuleCollider capsule = teleportZone.gameObject.AddComponent<CapsuleCollider>();
                capsule.isTrigger = true;
                capsule.height = 2f;
                capsule.radius = 0.5f;
                capsule.center = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning("TeleportZone не найден, чтобы добавить коллайдер.");
        }

        NavMeshSurface surface = FindObjectOfType<NavMeshSurface>();
        if (surface != null)
            surface.BuildNavMesh();
    }

    public void GoToAntHill()
    {
        if (lastPlacedAnthill == null || antMovement == null) return;

        Transform entrance = lastPlacedAnthill.transform.Find("TeleportZone");
        if (entrance != null)
        {
            antMovement.MoveTo(entrance.position);
        }
        else
        {
            Debug.LogWarning("TeleportZone не найден в муравейнике.");
        }
    }
}
