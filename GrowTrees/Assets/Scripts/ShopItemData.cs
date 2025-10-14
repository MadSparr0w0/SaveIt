using UnityEngine;

[System.Serializable]
public class ShopItemData
{
    public string itemName;
    public int cost;
    public GameObject prefab;
    public int width = 1;
    public int height = 1;
    public Sprite icon;

    public ShopItemData(string name, int itemCost, GameObject itemPrefab, int itemWidth, int itemHeight)
    {
        itemName = name;
        cost = itemCost;
        prefab = itemPrefab;
        width = itemWidth;
        height = itemHeight;
    }
}