using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI countText;
    
    public void SetItem(PlayerInventory.InventoryItem item)
    {
        if (iconImage != null && item.icon != null)
            iconImage.sprite = item.icon;
            
        if (nameText != null)
            nameText.text = item.displayName;
            
        if (countText != null)
            countText.text = item.count.ToString();
    }
}