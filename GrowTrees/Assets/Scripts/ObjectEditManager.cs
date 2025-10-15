using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectEditManager : MonoBehaviour
{
    [Header("Панель редактирования")]
    public GameObject editPanel;
    public Button rotateButton;
    public Button moveButton;
    public Button deleteButton;
    public Button confirmButton;
    public Button cancelButton;

    [Header("Материалы")]
    public Material validMaterial;
    public Material invalidMaterial;

    private PlaceableObject selectedObject;
    private GameObject movePreview;
    private bool isMoving = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private List<MapCell> originalCells = new List<MapCell>();

    public static ObjectEditManager Instance;

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
        if (editPanel != null)
            editPanel.SetActive(false);

        if (rotateButton != null)
            rotateButton.onClick.AddListener(RotateObject);

        if (moveButton != null)
            moveButton.onClick.AddListener(StartMoving);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(DeleteObject);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmMove);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelMove);
    }

    void Update()
    {
        if (isMoving && movePreview != null)
        {
            UpdateMovePreview();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                TryPlaceObject();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                CancelMove();
            }
        }
    }

    public void SelectObject(PlaceableObject obj)
    {
        if (isMoving || ShopManager.Instance.isShopOpen) return;

        selectedObject = obj;
        ShowEditPanel();
    }

    void ShowEditPanel()
    {
        if (editPanel == null || selectedObject == null) return;

        editPanel.SetActive(true);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
        editPanel.transform.position = screenPos + new Vector3(0, 100f, 0);
    }

    void HideEditPanel()
    {
        if (editPanel != null)
            editPanel.SetActive(false);

        selectedObject = null;
    }

    void RotateObject()
    {
        if (selectedObject != null)
        {
            selectedObject.transform.Rotate(0, 90, 0);
            Debug.Log($"Объект {selectedObject.objectName} повернут");
        }
    }

    void StartMoving()
    {
        if (selectedObject == null) return;

        isMoving = true;
        originalPosition = selectedObject.transform.position;
        originalRotation = selectedObject.transform.rotation;

        StoreOriginalCells();

        ClearCurrentCells();

        movePreview = Instantiate(selectedObject.gameObject);

        PlaceableObject previewPlaceable = movePreview.GetComponent<PlaceableObject>();
        if (previewPlaceable != null)
        {
            previewPlaceable.enabled = false;
            if (previewPlaceable.coinIndicator != null)
                previewPlaceable.coinIndicator.SetActive(false);
        }

        Collider[] colliders = movePreview.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        selectedObject.gameObject.SetActive(false);

        HideEditPanel();

        Debug.Log("Режим перемещения: используйте мышь для позиционирования, ЛКМ для подтверждения, ПКМ для отмены");
    }

    void StoreOriginalCells()
    {
        originalCells.Clear();

        Vector3 currentPos = selectedObject.transform.position;
        int currentX = Mathf.RoundToInt(currentPos.x);
        int currentZ = Mathf.RoundToInt(currentPos.z);

        for (int x = currentX; x < currentX + selectedObject.width; x++)
        {
            for (int z = currentZ; z < currentZ + selectedObject.height; z++)
            {
                MapCell cell = MapManager.Instance.GetCellAtPosition(x, z);
                if (cell != null)
                {
                    originalCells.Add(cell);
                }
            }
        }
    }

    void ClearCurrentCells()
    {
        foreach (MapCell cell in originalCells)
        {
            if (cell != null)
            {
                cell.RemoveObject();
            }
        }
    }

    void RestoreOriginalCells()
    {
        foreach (MapCell cell in originalCells)
        {
            if (cell != null && selectedObject != null)
            {
                cell.PlaceObject(selectedObject.gameObject);
            }
        }
    }

    void UpdateMovePreview()
    {
        if (movePreview == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 gridPosition = new Vector3(
                Mathf.Round(hit.point.x),
                0,
                Mathf.Round(hit.point.z)
            );

            movePreview.transform.position = gridPosition;

            bool isValid = CheckMoveValidity(gridPosition);
            Material materialToUse = isValid ? validMaterial : invalidMaterial;
            SetPreviewMaterial(movePreview, materialToUse);
        }
    }

    bool CheckMoveValidity(Vector3 position)
    {
        if (selectedObject == null) return false;

        int gridX = Mathf.RoundToInt(position.x);
        int gridZ = Mathf.RoundToInt(position.z);

        for (int x = gridX; x < gridX + selectedObject.width; x++)
        {
            for (int z = gridZ; z < gridZ + selectedObject.height; z++)
            {
                MapCell cell = MapManager.Instance.GetCellAtPosition(x, z);
                if (cell == null || !cell.isUnlocked || cell.placedObject != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void TryPlaceObject()
    {
        if (!isMoving || movePreview == null || selectedObject == null) return;

        Vector3 newPosition = movePreview.transform.position;

        if (CheckMoveValidity(newPosition))
        {
            selectedObject.transform.position = newPosition;
            selectedObject.transform.rotation = movePreview.transform.rotation;

            OccupyNewCells(newPosition);

            selectedObject.gameObject.SetActive(true);

            Debug.Log($"Объект {selectedObject.objectName} перемещен на новую позицию");

            CleanupMove();
        }
        else
        {
            Debug.Log("Невозможно разместить здесь! Выберите другое место.");
        }
    }

    void OccupyNewCells(Vector3 position)
    {
        int gridX = Mathf.RoundToInt(position.x);
        int gridZ = Mathf.RoundToInt(position.z);

        for (int x = gridX; x < gridX + selectedObject.width; x++)
        {
            for (int z = gridZ; z < gridZ + selectedObject.height; z++)
            {
                MapCell cell = MapManager.Instance.GetCellAtPosition(x, z);
                if (cell != null)
                {
                    cell.PlaceObject(selectedObject.gameObject);
                }
            }
        }
    }

    void ConfirmMove()
    {
        TryPlaceObject();
    }

    void CancelMove()
    {
        if (!isMoving || selectedObject == null) return;

        selectedObject.transform.position = originalPosition;
        selectedObject.transform.rotation = originalRotation;
        selectedObject.gameObject.SetActive(true);

        RestoreOriginalCells();

        CleanupMove();
        Debug.Log("Перемещение отменено");
    }

    void CleanupMove()
    {
        if (movePreview != null)
            Destroy(movePreview);

        isMoving = false;
        movePreview = null;
        originalCells.Clear();
    }

    void DeleteObject()
    {
        if (selectedObject == null) return;

        int refund = selectedObject.cost / 2;
        if (refund > 0)
        {
            GameManager.Instance.AddGreenPoints(refund);
            Debug.Log($"Объект удален. Возвращено {refund} GP");
        }

        ClearCurrentCells();

        Destroy(selectedObject.gameObject);

        HideEditPanel();
        Debug.Log($"Объект {selectedObject.objectName} удален");
    }

    void SetPreviewMaterial(GameObject obj, Material material)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = new Material(material);
        }
    }

    public bool IsEditing()
    {
        return isMoving || (selectedObject != null && editPanel != null && editPanel.activeInHierarchy);
    }
}