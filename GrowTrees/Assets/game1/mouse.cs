using UnityEngine;

public class PuzzlePieceFixed : MonoBehaviour
{
    [Header("Настройки")]
    public float snapDistance = 1.0f;

    [Header("Ссылки")]
    public GameObject targetSlot;

    private bool isDragging = false;
    private bool isLocked = false;
    private Vector3 dragOffset;
    private Camera mainCamera;
    private Vector3 originalPosition;

    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;

        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        Debug.Log($"{name} инициализирован. Камера: {mainCamera?.name}");
    }

    void OnMouseDown()
    {
        if (isLocked) return;

        isDragging = true;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorldPos;

        Debug.Log($"Начали перетаскивать {name}. Offset: {dragOffset}");
    }

    void OnMouseDrag()
    {
        if (!isDragging || isLocked) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        transform.position = mouseWorldPos + dragOffset;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        if (targetSlot != null && !isLocked)
        {
            CheckAndSnapToTarget();
        }
    }

    Vector3 GetMouseWorldPosition()
    {

        Vector3 mousePos = Input.mousePosition;

        if (mainCamera.orthographic)
        {
            Vector3 worldPoint = mainCamera.ScreenToWorldPoint(mousePos);
            worldPoint.z = transform.position.z;
            return worldPoint;
        }
        else
        {

            mousePos.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
            return mainCamera.ScreenToWorldPoint(mousePos);
        }
    }

    void CheckAndSnapToTarget()
    {
        float distance = Vector3.Distance(transform.position, targetSlot.transform.position);

        if (distance <= snapDistance)
        {

            transform.position = targetSlot.transform.position;
            isLocked = true;

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            Debug.Log($"✅ {name} закреплен на месте!");

            CheckPuzzleCompletion();
        }
    }

    void CheckPuzzleCompletion()
    {
        PuzzlePieceFixed[] allPieces = FindObjectsByType<PuzzlePieceFixed>(FindObjectsSortMode.None);
        int completedCount = 0;

        foreach (PuzzlePieceFixed piece in allPieces)
        {
            if (piece.isLocked) completedCount++;
        }

        Debug.Log($"Прогресс: {completedCount}/{allPieces.Length}");

        if (completedCount >= allPieces.Length)
        {
            Debug.Log("🎉 ВЕСЬ ПАЗЛ СОБРАН! УРОВЕНЬ ЗАВЕРШЕН!");
            OnPuzzleComplete();
        }
    }

    void OnPuzzleComplete()
    {

    }

    public void ResetPiece()
    {
        transform.position = originalPosition;
        isLocked = false;
        isDragging = false;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }
}
