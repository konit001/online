using UnityEngine;

[ExecuteAlways]
public class PerObjectOutline : MonoBehaviour
{
    public Color outlineColor = Color.white;
    public float outlineSize = 1.1f;
    public bool showOutline = true;

    private Renderer rend;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    void OnEnable() => Apply();
    void OnValidate() => Apply();

    void Apply()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        rend.GetPropertyBlock(mpb);

        mpb.SetColor("_OutlineColor", outlineColor);   // ชื่อต้องตรงกับ property ใน Shader Graph
        mpb.SetFloat("_OutlineSize", outlineSize);
        mpb.SetFloat("_ShowOutline", showOutline ? 1f : 0f);

        rend.SetPropertyBlock(mpb);
    }
}