using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text itemNameText;
    public Text costText;
    public Text sizeText;
    public Image itemIcon;
    public Button purchaseButton;

    private ShopItemData itemData;

    public void Initialize(ShopItemData data)
    {
        itemData = data;

        if (itemNameText == null) Debug.LogError("ItemNameText не назначен!", this);
        if (costText == null) Debug.LogError("CostText не назначен!", this);
        if (sizeText == null) Debug.LogError("SizeText не назначен!", this);
        if (purchaseButton == null) Debug.LogError("PurchaseButton не назначен!", this);

        if (itemNameText != null) itemNameText.text = data.itemName;
        if (costText != null) costText.text = $"{data.cost} GP";
        if (sizeText != null) sizeText.text = $"{data.width}x{data.height}";
        if (itemIcon != null && data.icon != null) itemIcon.sprite = data.icon;

        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(OnPurchaseClick);
        }

        UpdatePurchaseButton();
    }

    public void UpdatePurchaseButton()
    {
        if (purchaseButton == null)
        {
            Debug.LogError("PurchaseButton is NULL!", this);
            return;
        }

        if (itemData == null)
        {
            Debug.LogError("ItemData is NULL! ¬озможно товар не инициализирован", this);
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL!", this);
            return;
        }

        purchaseButton.interactable = GameManager.Instance.greenPoints >= itemData.cost;
    }

    void OnPurchaseClick()
    {
        if (itemData == null)
        {
            Debug.LogError("Ќельз€ купить - ItemData is NULL!", this);
            return;
        }

        ShopManager.Instance.PurchaseItem(itemData);
    }
}