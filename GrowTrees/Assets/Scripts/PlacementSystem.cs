using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [Header("Настройки размещения")]
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    private GameObject currentObjectToPlace;
    private GameObject placementPreview;
    private ShopItemData currentItemData;
    private bool isPlacing = false;

    public static PlacementSystem Instance;

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

    void Update()
    {
        if (!isPlacing) return;

        UpdatePreviewPosition();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceObject();
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    public void StartPlacingObject(ShopItemData itemData, GameObject objectPrefab)
    {
        currentItemData = itemData;
        currentObjectToPlace = objectPrefab;
        isPlacing = true;

        placementPreview = Instantiate(objectPrefab);
        SetPreviewMaterial(placementPreview, validPlacementMaterial);

        Collider[] colliders = placementPreview.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        Debug.Log($"Режим размещения: {itemData.itemName}");
    }

    void UpdatePreviewPosition()
    {
        if (placementPreview == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 gridPosition = new Vector3(
                Mathf.Round(hit.point.x),
                0,
                Mathf.Round(hit.point.z)
            );

            placementPreview.transform.position = gridPosition;

            bool isValid = CheckPlacementValidity(gridPosition);
            Material materialToUse = isValid ? validPlacementMaterial : invalidPlacementMaterial;
            SetPreviewMaterial(placementPreview, materialToUse);
        }
    }

    bool CheckPlacementValidity(Vector3 position)
    {
        int gridX = Mathf.RoundToInt(position.x);
        int gridZ = Mathf.RoundToInt(position.z);

        for (int x = gridX; x < gridX + currentItemData.width; x++)
        {
            for (int z = gridZ; z < gridZ + currentItemData.height; z++)
            {
                MapCell cell = MapManager.Instance.GetCellAtPosition(x, z);
                if (cell == null || !cell.isUnlocked || cell.placedObject != null)
                    return false;
            }
        }
        return true;
    }

    void TryPlaceObject()
    {
        if (placementPreview == null) return;

        Vector3 position = placementPreview.transform.position;

        if (CheckPlacementValidity(position))
        {
            GameObject placedObject = Instantiate(currentObjectToPlace, position, Quaternion.identity);

            int gridX = Mathf.RoundToInt(position.x);
            int gridZ = Mathf.RoundToInt(position.z);

            for (int x = gridX; x < gridX + currentItemData.width; x++)
            {
                for (int z = gridZ; z < gridZ + currentItemData.height; z++)
                {
                    MapCell cell = MapManager.Instance.GetCellAtPosition(x, z);
                    if (cell != null)
                    {
                        cell.PlaceObject(placedObject);
                    }
                }
            }

            PlaceableObject placeable = placedObject.GetComponent<PlaceableObject>();
            if (placeable == null)
                placeable = placedObject.AddComponent<PlaceableObject>();

            placeable.objectName = currentItemData.itemName;
            placeable.cost = currentItemData.cost;
            placeable.width = currentItemData.width;
            placeable.height = currentItemData.height;

            if (currentItemData.itemName.Contains("Дуб"))
            {
                placeable.generatesCoins = true;
                placeable.coinReward = 5;
                placeable.generationTime = 60f;
            }

            Debug.Log($"Объект {currentItemData.itemName} размещен!");
            CancelPlacement();
        }
        else
        {
            Debug.Log("Невозможно разместить здесь!");
        }
    }

    void CancelPlacement()
    {
        if (placementPreview != null)
            Destroy(placementPreview);

        currentObjectToPlace = null;
        currentItemData = null;
        isPlacing = false;

        Debug.Log("Режим размещения отменен");
    }

    void SetPreviewMaterial(GameObject obj, Material material)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }
    }
}