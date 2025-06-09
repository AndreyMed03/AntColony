using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiggingManager : MonoBehaviour
{
    private GameObject selectedPiece;
    private AntMovement antMovement;
    private NavMeshBaker navMeshBaker;

    private bool isDigging = false;
    private int ignoreLayerMask;

    void Start()
    {
        antMovement = FindObjectOfType<AntMovement>();
        navMeshBaker = FindObjectOfType<NavMeshBaker>();
        
        // Игнорируем слой Transparent Piece
        int transparentLayer = LayerMask.NameToLayer("Transparent Piece");
        ignoreLayerMask = ~(1 << transparentLayer);

        foreach (Button diggingBtn in Resources.FindObjectsOfTypeAll<Button>())
        {
            if (diggingBtn.name == "Digging_Button")
                diggingBtn.onClick.AddListener(ToggleDiggingMode);
        }
    }

    void Update()
    {
        if (antMovement == null)
            antMovement = FindObjectOfType<AntMovement>();

        if (!isDigging || EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ignoreLayerMask))
            {
                GameObject hitObj = hit.collider.gameObject;
                Debug.Log($"[DiggingManager] Raycast hit: {hitObj.name}, Tag: {hitObj.tag}, Layer: {hitObj.layer}, InstanceID: {hitObj.GetInstanceID()}");

                if (hitObj.CompareTag("Piece"))
                {
                    Debug.Log("[DiggingManager] Hit object has tag 'Piece'");

                    if (selectedPiece != null)
                    {
                        Debug.Log($"[DiggingManager] selectedPiece: {selectedPiece.name}, InstanceID: {selectedPiece.GetInstanceID()}");
                    }

                    if (selectedPiece == hitObj)
                    {
                        Debug.Log("[DiggingManager] Piece selected again, destroying it");
                        Destroy(hitObj);
                        selectedPiece = null;
                        navMeshBaker?.Bake();
                        ToggleDiggingMode();
                    }
                    else
                    {
                        selectedPiece = hitObj;
                        //HighlightPiece(selectedPiece);
                    }
                }
            }
        }
    }

    public void ToggleDiggingMode()
    {
        isDigging = !isDigging;
        antMovement.SetPlacementMode(isDigging);
        //ResetHighlight();
    }

    private void HighlightPiece(GameObject piece)
    {
        if (piece.GetComponent<OutlineEffect>() == null)
        {
            var outline = piece.AddComponent<OutlineEffect>();
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 0.03f;
        }
    }

    private void ResetHighlight()
    {
        if (selectedPiece != null)
        {
            var outline = selectedPiece.GetComponent<OutlineEffect>();
            if (outline != null)
                Destroy(outline);

            selectedPiece = null;
        }
    }
}
