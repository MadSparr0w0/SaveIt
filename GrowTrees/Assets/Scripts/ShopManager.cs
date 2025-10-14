using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject shopPanel;
    public Button shopToggleButton;

    [Header("������ - ���������� ������� ShopItem �����")]
    public List<ShopItemUI> shopItems = new List<ShopItemUI>();

    [Header("������ �������")]
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
        Debug.Log("ShopManager Start ������");

        if (shopPanel != null)
            shopPanel.SetActive(false);
        else
            Debug.LogError("ShopPanel �� ��������!");

        if (shopToggleButton != null)
        {
            shopToggleButton.onClick.AddListener(ToggleShop);
        }
        else
        {
            Debug.LogError("ShopToggleButton �� ��������!");
        }

        InitializeShopItems();
    }

    public void ToggleShop()
    {
        Debug.Log("ToggleShop ������");

        isShopOpen = !isShopOpen;

        if (shopPanel != null)
        {
            shopPanel.SetActive(isShopOpen);
            Debug.Log($"������� {(isShopOpen ? "������" : "������")}");
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
        Debug.Log($"������������� �������: {availableItems.Count} ��������, {shopItems.Count} UI ���������");

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
        Debug.Log("UpdateShopItems ������");

        foreach (ShopItemUI itemUI in shopItems)
        {
            if (itemUI != null)
            {
                itemUI.UpdatePurchaseButton();
            }
            else
            {
                Debug.LogError("������ NULL ������� � shopItems!");
            }
        }
    }

    public void PurchaseItem(ShopItemData item)
    {
        Debug.Log($"������� �������: {item.itemName} �� {item.cost} GP");

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is NULL!");
            return;
        }

        if (GameManager.Instance.SpendGreenPoints(item.cost))
        {
            Debug.Log($"������� ������: {item.itemName}");

            if (PlacementSystem.Instance == null)
            {
                Debug.LogError("PlacementSystem.Instance is NULL!");
                return;
            }

            if (item.prefab == null)
            {
                Debug.LogError($"Prefab ��� {item.itemName} is NULL!");
                return;
            }

            PlacementSystem.Instance.StartPlacingObject(item, item.prefab);
        }
        else
        {
            Debug.Log("������������ Green Points!");
        }
    }
}