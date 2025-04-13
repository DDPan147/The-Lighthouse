using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [System.Serializable]
    public class InventoryItem
    {
        public ItemType type;
        public string displayName;
        public Sprite icon;
        public int count;
    }

    [Header("Inventario")]
    public List<InventoryItem> items = new List<InventoryItem>();
    
    [Header("UI")]
    public GameObject inventoryPanel;
    public TextMeshProUGUI inventoryText;
    public GameObject itemPrefab; // Prefab para mostrar cada ítem en la UI
    public Transform itemContainer; // Contenedor donde se instanciarán los prefabs
    
    [Header("Configuración")]
    public KeyCode toggleInventoryKey = KeyCode.I;
    private bool isInventoryOpen = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        // Inicializar el inventario con los tipos de ítems
        InitializeInventory();
    }
    
    private void Start()
    {
        // Cerrar el inventario al inicio
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }
    
    private void Update()
    {
        // Abrir/cerrar inventario con tecla
        if (Input.GetKeyDown(toggleInventoryKey))
        {
            ToggleInventory();
        }
    }
    
    private void InitializeInventory()
    {
        // Crear entradas para cada tipo de ítem
        items.Add(new InventoryItem { 
            type = ItemType.TableLeg, 
            displayName = "Pata de Mesa", 
            count = 0 
        });
        
        items.Add(new InventoryItem { 
            type = ItemType.ClockGear, 
            displayName = "Engranaje de Reloj", 
            count = 0 
        });
        
        items.Add(new InventoryItem { 
            type = ItemType.DollPart, 
            displayName = "Pieza de Muñeco", 
            count = 0 
        });
    }
    
    public void AddItem(ItemType type, int amount = 1)
    {
        // Buscar el ítem en el inventario
        InventoryItem item = items.Find(i => i.type == type);
        
        if (item != null)
        {
            item.count += amount;
            
            // Actualizar el contador en MinigameFourManager
            UpdateMinigameManager(type, item.count);
            
            // Actualizar la UI
            UpdateInventoryUI();
            
            Debug.Log($"Añadido {amount} {item.displayName}(s). Total: {item.count}");
        }
    }
    
    public int GetItemCount(ItemType type)
    {
        InventoryItem item = items.Find(i => i.type == type);
        return item != null ? item.count : 0;
    }
    
    public bool HasEnoughItems(ItemType type, int requiredAmount)
    {
        return GetItemCount(type) >= requiredAmount;
    }
    
    private void UpdateMinigameManager(ItemType type, int newCount)
    {
        // Actualizar el contador correspondiente en el MinigameFourManager
        if (MinigameFourManager.Instance != null)
        {
            switch (type)
            {
                case ItemType.TableLeg:
                    MinigameFourManager.Instance.tableLegsCollected = newCount;
                    break;
                case ItemType.ClockGear:
                    MinigameFourManager.Instance.clockGearsCollected = newCount;
                    break;
                case ItemType.DollPart:
                    MinigameFourManager.Instance.dollPartsCollected = newCount;
                    break;
            }
        }
    }
    
    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
            
            // Si se abre el inventario, actualizar la UI
            if (isInventoryOpen)
            {
                UpdateInventoryUI();
                
                // Desbloquear el cursor cuando el inventario está abierto
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Volver a bloquear el cursor cuando se cierra el inventario
                // (solo si el juego está en modo de juego normal)
                CameraController cameraController = FindObjectOfType<CameraController>();
                if (cameraController != null)
                {
                    cameraController.LockCursor();
                }
            }
        }
    }
    
    private void UpdateInventoryUI()
    {
        // Actualizar texto simple si está configurado
        if (inventoryText != null)
        {
            string text = "Inventario:\n";
            foreach (InventoryItem item in items)
            {
                if (item.count > 0)
                {
                    text += $"{item.displayName}: {item.count}\n";
                }
            }
            inventoryText.text = text;
        }
        
        // Actualizar UI visual si está configurada
        if (itemContainer != null && itemPrefab != null)
        {
            // Limpiar contenedor
            foreach (Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Crear elementos de UI para cada ítem
            foreach (InventoryItem item in items)
            {
                if (item.count > 0)
                {
                    GameObject itemUI = Instantiate(itemPrefab, itemContainer);
                    
                    // Configurar el elemento de UI
                    InventoryItemUI uiScript = itemUI.GetComponent<InventoryItemUI>();
                    if (uiScript != null)
                    {
                        uiScript.SetItem(item);
                    }
                }
            }
        }
    }
}