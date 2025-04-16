using System.Collections.Generic;
using UnityEngine;

public class ItemDisplayTable : MonoBehaviour
{
    [System.Serializable]
    public class ItemVisual
    {
        public ItemType itemType;
        public List<GameObject> visualObjects = new List<GameObject>();
    }

    [Header("Item Visuals")]
    public List<ItemVisual> itemVisuals = new List<ItemVisual>();
    
    [Header("References")]
    public MinigameFourManager minigameManager;

    // Diccionario para rastrear cuántos objetos de cada tipo están activos
    private Dictionary<ItemType, int> activeCount = new Dictionary<ItemType, int>();

    private void Start()
    {
        if (minigameManager == null)
            minigameManager = MinigameFourManager.Instance;
        
        // Inicializar el diccionario y desactivar todos los objetos visuales
        foreach (ItemVisual item in itemVisuals)
        {
            // Inicializar contador a 0 para cada tipo
            activeCount[item.itemType] = 0;
            
            // Desactivar todos los objetos visuales al inicio
            foreach (GameObject obj in item.visualObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
        
        Debug.Log("ItemDisplayTable inicializado correctamente");
    }
    
    // Método público que se llama desde MinigameFourManager
    public void OnItemCollected(ItemType type)
    {
        Debug.Log($"OnItemCollected llamado: {type}");
        
        // Buscar el ItemVisual correspondiente al tipo
        ItemVisual targetVisual = null;
        
        // Buscar manualmente para evitar problemas de comparación de enums
        foreach (var visual in itemVisuals)
        {
            if (visual.itemType.ToString() == type.ToString())
            {
                targetVisual = visual;
                break;
            }
        }
        
        if (targetVisual == null)
        {
            Debug.LogError($"No se encontró ItemVisual para el tipo {type}");
            return;
        }
        
        // Obtener el contador actual para este tipo
        int currentCount = activeCount[type];
        
        // Verificar si hay más objetos para activar
        if (currentCount < targetVisual.visualObjects.Count)
        {
            // Activar el siguiente objeto en la secuencia
            GameObject objToActivate = targetVisual.visualObjects[currentCount];
            
            if (objToActivate != null)
            {
                objToActivate.SetActive(true);
                Debug.Log($"Activado {objToActivate.name} para {type} (#{currentCount+1})");
                
                // Incrementar el contador
                activeCount[type] = currentCount + 1;
            }
            else
            {
                Debug.LogWarning($"Objeto visual en posición {currentCount} es NULL");
            }
        }
        else
        {
            Debug.LogWarning($"Ya se han activado todos los objetos visuales para {type} ({currentCount}/{targetVisual.visualObjects.Count})");
        }
    }
    
    // Método para pruebas desde el Editor
    public void TestActivateVisual(string typeStr)
    {
        ItemType type;
        if (System.Enum.TryParse(typeStr, out type))
        {
            OnItemCollected(type);
        }
    }
}