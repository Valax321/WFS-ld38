using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Color outlineColor;
    Color oldOutlineColor;
    [Range(0f, 0.1f)]
    public float width;
    Material mat;
    MeshRenderer mesh;

    void Awake()
    {
        mat = new Material(Shader.Find("Outlined/Silhouette Only"));
        mesh = GetComponent<MeshRenderer>();
        List<Material> mats = new List<Material>();
        mats.AddRange(mesh.materials);
        mats.Add(mat);
        mesh.materials = mats.ToArray();
                
        mat.SetFloat("_Outline", width);
    }

    void Update()
    {
        if (outlineColor != oldOutlineColor)
        {
            oldOutlineColor = outlineColor;
            mat.SetColor("_OutlineColor", outlineColor);
        }
    }
}
