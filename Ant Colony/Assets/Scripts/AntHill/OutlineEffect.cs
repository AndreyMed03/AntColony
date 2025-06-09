using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineEffect : MonoBehaviour
{
    public Color OutlineColor = Color.yellow;
    public float OutlineWidth = 0.03f;

    private Material outlineMat;
    private Renderer rend;

    // Название Shader Graph материала (укажи правильное имя)
    private const string OutlineShaderName = "Shader Graphs/Outline Shader Graph"; // <-- Поменяй на своё

    void OnEnable()
    {
        rend = GetComponent<Renderer>();

        Shader outlineShader = Shader.Find(OutlineShaderName);
        if (outlineShader == null)
        {
            Debug.LogError($"[OutlineEffect] Shader '{OutlineShaderName}' not found! Check shader path.");
            return;
        }

        outlineMat = new Material(outlineShader);
        outlineMat.SetColor("OutlineColor", OutlineColor);
        outlineMat.SetFloat("OutlineWidth", OutlineWidth);

        // Добавляем как дополнительный материал (для обводки)
        var mats = new Material[rend.sharedMaterials.Length + 1];
        rend.sharedMaterials.CopyTo(mats, 0);
        mats[mats.Length - 1] = outlineMat;
        rend.materials = mats;
    }

    void OnDisable()
    {
        if (rend != null && outlineMat != null)
        {
            var mats = new System.Collections.Generic.List<Material>(rend.sharedMaterials);
            // Удаляем все материалы, использующие наш Shader
            mats.RemoveAll(m => m.shader == outlineMat.shader);
            rend.materials = mats.ToArray();
        }

        if (outlineMat != null)
        {
            Destroy(outlineMat);
            outlineMat = null;
        }
    }
}
