using UnityEngine;

public class MapCell : MonoBehaviour
{
    [Header("Координаты в сетке")]
    public int gridX;
    public int gridY;

    [Header("Состояние")]
    public bool isUnlocked = false;
    public GameObject placedObject;
    public LandSection parentSection;

    [Header("Материалы")]
    public Material lockedMaterial;
    public Material unlockedMaterial;

    private Renderer cellRenderer;
    private float lastClickTime = 0f;
    private const float minClickInterval = 0.5f;

    void Start()
    {
        cellRenderer = GetComponent<Renderer>();

        if (gridX == 0 && gridY == 0)
        {
            gridX = Mathf.RoundToInt(transform.position.x);
            gridY = Mathf.RoundToInt(transform.position.z);
        }

        UpdateVisual();
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateVisual();
    }

    public void Lock()
    {
        isUnlocked = false;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (cellRenderer != null)
        {
            if (isUnlocked && unlockedMaterial != null)
                cellRenderer.material = unlockedMaterial;
            else if (!isUnlocked && lockedMaterial != null)
                cellRenderer.material = lockedMaterial;
        }
    }

    public bool CanPlaceObject()
    {
        return isUnlocked && placedObject == null;
    }

    public void PlaceObject(GameObject obj)
    {
        placedObject = obj;
    }

    public void RemoveObject()
    {
        placedObject = null;
    }

    void OnMouseDown()
    {
        if (Time.time - lastClickTime < minClickInterval) return;
        lastClickTime = Time.time;

        if (ShopManager.Instance != null && ShopManager.Instance.isShopOpen)
        {
            return;
        }

        if (!isUnlocked && parentSection != null && LandPurchaseManager.Instance != null)
        {
            LandPurchaseManager.Instance.OnCellClicked(this);
        }
    }
}