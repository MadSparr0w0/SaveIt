using UnityEngine;
using System.Collections;

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

    private static int lockedPiecesCount = 0;
    private static int totalPiecesCount = 0;

    void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;

        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        totalPiecesCount++;

        Debug.Log($"{name} инициализирован. Камера: {mainCamera?.name}. Всего элементов: {totalPiecesCount}");
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

            lockedPiecesCount++;
            Debug.Log($"Заблокировано элементов: {lockedPiecesCount}/{totalPiecesCount}");

            PuzzleGameManager gameManager = FindObjectOfType<PuzzleGameManager>();
            if (gameManager != null)
            {
                gameManager.OnPieceLocked();
            }

            CheckPuzzleCompletion();
        }
    }

    void CheckPuzzleCompletion()
    {
        if (lockedPiecesCount >= totalPiecesCount)
        {
            Debug.Log($"🎉 ВЕСЬ ПАЗЛ СОБРАН! Заблокировано: {lockedPiecesCount}/{totalPiecesCount}");
        }
        else
        {
            Debug.Log($"Осталось элементов: {totalPiecesCount - lockedPiecesCount}");
        }
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    public void ResetPiece()
    {
        if (isLocked)
        {
            lockedPiecesCount--;
        }

        transform.position = originalPosition;
        isLocked = false;
        isDragging = false;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    public static void ResetCounters()
    {
        lockedPiecesCount = 0;
        totalPiecesCount = 0;
    }

    public static void SetTotalPiecesCount(int count)
    {
        totalPiecesCount = count;
    }
}