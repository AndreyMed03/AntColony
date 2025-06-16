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
    public LayerMask placementLayerMask;
    public LayerMask blockPlacementMask;

    private GameObject currentGhost;
    private GameObject lastPlacedAnthill;

    private NavMeshSurface navMeshSurface;

    private Camera mainCamera;
    private bool isPlacing = false;
    private bool isDragging = false;
    private bool hasRotated = false;

    private AntMovement antMovement;
    private CameraRotation cameraRotation;

    void Start()
    {
        mainCamera = Camera.main;
        applyButton.SetActive(false);
        cancelButton.SetActive(false);
        goToAntHillButton.SetActive(false);

        antMovement = FindObjectOfType<AntMovement>();
        cameraRotation = FindObjectOfType<CameraRotation>();

    }

    public void StartPlacement()
    {
        if (antMovement == null) return;
        if (isPlacing) return;

        isPlacing = true;
        antMovement.SetPlacementMode(true);
        cameraRotation.isRotationBlocked = true;

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
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
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

        // Сначала проверим, не попали ли по запрещённым слоям
        if (Physics.Raycast(ray, out RaycastHit blockHit, Mathf.Infinity, blockPlacementMask))
        {
            // Если запрещённый слой ближе, чем разрешённый — выходим
            if (Physics.Raycast(ray, out RaycastHit allowedHit, Mathf.Infinity, placementLayerMask))
            {
                if (blockHit.distance < allowedHit.distance)
                    return; // Заблокирован — не двигаем
            }
            else
            {
                return; // Попали только в запрещённый слой — блокируем
            }
        }

        // Теперь размещаем по разрешённым слоям
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayerMask))
        {
            currentGhost.transform.position = hit.point;

            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            currentGhost.transform.rotation = slopeRotation * Quaternion.Euler(0f, currentGhost.transform.eulerAngles.y, 0f);
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

        FindObjectOfType<AntHillChunkManager>()?.HandleAnthillPlacement(currentGhost);
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
        cameraRotation.isRotationBlocked = false;
        hasRotated = false;
    }

    private void SetTransparent(GameObject obj, float alpha)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.color;
                    color.a = alpha;
                    mat.color = color;
                }

                if (alpha < 1f)
                {
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                }
                else
                {
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = -1;
                }
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

        // Запекаем карту с новым префабом
        GameObject navMeshObj = GameObject.Find("NavMesh");
        if (navMeshObj != null)
        {
            navMeshSurface = navMeshObj.GetComponent<NavMeshSurface>();
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning("Не найден объект с именем NavMesh");
        }
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
