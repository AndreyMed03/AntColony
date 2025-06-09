using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraZoneCleaning : MonoBehaviour
{
    [SerializeField] private string targetTag = "Ant";
    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private string chunkLayerName = "Chunk Piece";           // изначальный слой кусков
    [SerializeField] private string transparentLayerName = "Transparent Piece"; // временный прозрачный слой
    [SerializeField] private float transparentAlpha = 0.1f;
    [SerializeField] private float fadeSpeed = 10f;
    [SerializeField] private float eliminateRadius = 2f;

    private int chunkLayer;
    private int transparentLayer;
    private Transform target;
    private float searchTimer;

    private Dictionary<Renderer, float> fadingRenderers = new Dictionary<Renderer, float>();

    void Awake()
    {
        chunkLayer = LayerMask.NameToLayer(chunkLayerName);
        transparentLayer = LayerMask.NameToLayer(transparentLayerName);
    }

    void Update()
    {
        if (target == null)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0f)
            {
                GameObject found = GameObject.FindWithTag(targetTag);
                if (found != null)
                {
                    target = found.transform;
                    Debug.Log("Ant target found by CameraZoneCleaning.");
                }
                searchTimer = 0.5f;
            }
            return;
        }

        HandleOcclusion();
        FadeAll();
    }

    private void HandleOcclusion()
    {
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        Ray ray = new Ray(transform.position, direction.normalized);
        RaycastHit[] hits = Physics.SphereCastAll(ray, checkRadius, distance);

        HashSet<Renderer> currentHits = new HashSet<Renderer>();

        foreach (RaycastHit hit in hits)
        {
            Renderer r = hit.collider.GetComponent<Renderer>();

            if (r != null && (r.gameObject.layer == chunkLayer || r.gameObject.layer == transparentLayer))
            {
                // 👉 Добавляем фильтр по близости к муравью:
                float distToTarget = Vector3.Distance(hit.point, target.position);
                if (distToTarget < eliminateRadius) continue; // Пропускаем куски слишком близко к муравью

                currentHits.Add(r);

                if (!fadingRenderers.ContainsKey(r))
                {
                    fadingRenderers[r] = transparentAlpha;
                    r.gameObject.layer = transparentLayer;
                }
                else
                {
                    fadingRenderers[r] = transparentAlpha;
                }

                SetMaterialTransparent(r);
            }
        }

        List<Renderer> toReset = new List<Renderer>();
        foreach (var r in fadingRenderers.Keys)
        {
            if (!currentHits.Contains(r))
            {
                toReset.Add(r);
            }
        }

        foreach (var r in toReset)
        {
            fadingRenderers[r] = 1f;
        }
    }

    private void FadeAll()
    {
        List<Renderer> finished = new List<Renderer>();

        foreach (var kvp in fadingRenderers)
        {
            Renderer r = kvp.Key;
            float targetAlpha = kvp.Value;

            if (r == null) continue;

            Material mat = r.material;
            Color c = mat.color;
            float newAlpha = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            mat.color = new Color(c.r, c.g, c.b, newAlpha);

            if (Mathf.Abs(newAlpha - targetAlpha) < 0.01f)
            {
                mat.color = new Color(c.r, c.g, c.b, targetAlpha);

                if (targetAlpha == 1f)
                {
                    r.gameObject.layer = chunkLayer;
                    finished.Add(r);
                }
            }
        }

        foreach (var r in finished)
        {
            fadingRenderers.Remove(r);
        }
    }

    private void SetMaterialTransparent(Renderer renderer)
    {
        Material mat = renderer.material;
        if (mat.HasProperty("_Mode"))
        {
            mat.SetFloat("_Mode", 2); // Transparent mode
        }
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}


