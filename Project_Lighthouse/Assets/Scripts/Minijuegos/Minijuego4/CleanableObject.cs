using UnityEngine;
using UnityEngine.EventSystems;

public class CleanableObject : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float outlineScale = 1.05f;
    
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer && _meshRenderer.materials.Length > 1)
        {
            outlineMaterial = _meshRenderer.materials[1];
        }
        else
        {
            Debug.LogWarning("Missing required materials on " + gameObject.name);
        }
        SetOutlineVisibility(false);
    }

    public void SetOutlineVisibility(bool isVisible)
    {
        if (outlineMaterial != null)
        {
            Color color = outlineColor;
            color.a = isVisible ? 1f : 0f;
            float scale = isVisible ? outlineScale : 0f;
            
            outlineMaterial.SetColor("_Color", color);
            outlineMaterial.SetFloat("_Scale", scale);
        }
    }
}