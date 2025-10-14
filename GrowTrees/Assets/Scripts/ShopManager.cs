using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject shopPanel;
    public Button shopToggleButton;

    [Header("Товары - перетащите готовые ShopItem сцены")]
    public List<ShopItemUI> shopItems = new List<ShopItemUI>();

    [Header("Данные товаров")]
    public List<ShopItemData> availableItems = new List<ShopItemData>();

    public bool isShopOpen { get; private set; } = false;

    public static ShopManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("ShopManager Start вызван");

        if (shopPanel != null)
            shopPanel.SetActive(false);
        else
            Debug.LogError("ShopPanel не назначен!");

        if (shopToggleButton != null)
        {
            shopToggleButton.onClick.AddListener(ToggleShop);
        }
        else
        {
            Debug.LogError("ShopToggleButton не назначен!");
        }

        InitializeShopItems();
    }

    public void ToggleShop()
    {
        Debug.Log("ToggleShop вызван");

        isShopOpen = !isShopOpen;

        if (shopPanel != null)
        {
            shopPanel.SetActive(isShopOpen);
            Debug.Log($"Магазин {(isShopOpen ? "открыт" : "закрыт")}");
        }

        if (MapManager.Instance != null)
        {
            MapCell[] allCells = FindObjectsByType<MapCell>(FindObjectsSortMode.None);
            foreach (MapCell cell in allCells)
            {
                Collider collider = cell.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = !isShopOpen;
                }
            }
        }

        if (isShopOpen)
        {
            UpdateShopItems();
        }
    }

    void InitializeShopItems()
    {
        Debug.Log($"Инициализация товаров: {availableItems.Count} доступно, {shopItems.Count} UI элементов");

        for (int i = 0; i < Mathf.Min(availableItems.Count, shopItems.Count); i++)
        {
            if (shopItems[i] == null)
            {
                Debug.LogError($"ShopItems[{i}] is NULL!");
                continue;
            }

            if (availableItems[i] == null)
            {
                Debug.LogError($"AvailableItems[{i}] is NULL!");
                continue;
            }

            shopItems[i].Initialize(availableItems[i]);
        }
    }

    void UpdateShopItems()
    {
        Debug.Log("UpdateShopItems вызван");

        foreach (ShopItemUI itemUI in shopItems)
        {
            if (itemUI != null)
            {
                itemUI.UpdatePurchaseButton();
            }
            else
            {
                Debug.LogError("Найден NULL элемент в shopItems!");
            }
        }
    }

    public void PurchaseItem(ShopItemData item)
    {
        Debug.Log($"Попытка покупки: {item.itemName} за {item.cost} GP");

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL!");
            return;
        }

        if (GameManager.Instance.SpendGreenPoints(item.cost))
        {
            Debug.Log($"Успешно куплен: {item.itemName}");

            if (PlacementSystem.Instance == null)
            {
                Debug.LogError("PlacementSystem.Instance is NULL!");
                return;
            }

            if (item.prefab == null)
            {
                Debug.LogError($"Prefab для {item.itemName} is NULL!");
                return;
            }

            PlacementSystem.Instance.StartPlacingObject(item, item.prefab);
        }
        else
        {
            Debug.Log("Недостаточно Green Points!");
        }
    }
}