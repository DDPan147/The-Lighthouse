using UnityEngine;

public enum ItemType 
{
    TableLeg,
    ClockGear,
    DollPart
}

public class CleanableObject : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    
    [Header("Visual Feedback")]
    [SerializeField] private Material outlineMaterial;
    private MeshRenderer meshRenderer;
    
    [Header("Pickup Effects")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private ParticleSystem pickupParticles;
    
    private void Awake()
    {
        // Obtener el renderer y el material de outline si existe
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.materials.Length > 1)
        {
            outlineMaterial = meshRenderer.materials[1];
        }
        SetOutlineVisibility(false);
    }
    
    public void SetOutlineVisibility(bool isVisible)
    {
        if (outlineMaterial != null)
        {
            float outlineScale = outlineMaterial.GetFloat("_Scale");
            outlineScale = isVisible ? 1.05f : 0f;
            outlineMaterial.SetColor("_Color", Color.yellow);
            outlineMaterial.SetFloat("_Scale", outlineScale);
        }
    }
    
    // Método que se llamará cuando el objeto sea recogido
    public void OnCollected()
    {
        Debug.Log($"[CleanableObject] Objeto recogido: {itemType}");
    
        // Llamar al manager para añadir al inventario
        MinigameFourManager manager = MinigameFourManager.Instance;
        if (manager != null)
        {
            Debug.Log($"[CleanableObject] Llamando a AddItemToInventory: {itemType}");
            manager.AddItemToInventory(itemType);
        }
        else
        {
            Debug.LogError("[CleanableObject] No se encontró MinigameFourManager.Instance");
        }
    
        // Desactivar este objeto
        gameObject.SetActive(false);
    }
    
    private void PlayPickupEffects()
    {
        // Reproducir sonido
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        
        // Reproducir partículas
        if (pickupParticles != null)
        {
            ParticleSystem particles = Instantiate(pickupParticles, transform.position, Quaternion.identity);
            Destroy(particles.gameObject, particles.main.duration);
        }
    }
}