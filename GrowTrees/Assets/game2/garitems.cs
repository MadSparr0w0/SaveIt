using UnityEngine;
using System.Collections;

public class GarbageItem : MonoBehaviour
{
    [Header("Тип мусора")]
    public GarbageType garbageType;

    [Header("Настройки")]
    public float snapDistance = 1.0f;

    private bool isDragging = false;
    private bool isLocked = false;
    private Vector3 dragOffset;
    private Camera mainCamera;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;

    private static int lockedGarbageCount = 0;
    private static int totalGarbageCount = 0;

    public enum GarbageType
    {
        Plastic,
        Metal
    }

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;

        AutoDetectGarbageType();

        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        Debug.Log($"{name} инициализирован. Тип: {garbageType}");
    }

    void AutoDetectGarbageType()
    {
        if (name.ToLower().Contains("met"))
        {
            garbageType = GarbageType.Metal;
        }
        else if (name.ToLower().Contains("plas"))
        {
            garbageType = GarbageType.Plastic;
        }
        else
        {
            Debug.LogWarning($"Не удалось определить тип мусора для {name}. Установлен тип по умолчанию: Plastic");
            garbageType = GarbageType.Plastic;
        }
    }

    void OnMouseDown()
    {
        if (isLocked) return;

        isDragging = true;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorldPos;

        spriteRenderer.sortingOrder = 10;
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
        spriteRenderer.sortingOrder = 1;

        GarbageBin[] allBins = FindObjectsOfType<GarbageBin>();
        GarbageBin closestBin = null;
        float closestDistance = float.MaxValue;

        foreach (GarbageBin bin in allBins)
        {
            float distance = Vector3.Distance(transform.position, bin.transform.position);
            if (distance <= snapDistance && distance < closestDistance)
            {
                closestDistance = distance;
                closestBin = bin;
            }
        }

        if (closestBin != null)
        {
            if (closestBin.acceptedGarbageType == garbageType)
            {
                LockToBin(closestBin);
            }
            else
            {
                transform.position = originalPosition;
                Debug.Log($"❌ Ошибка! {garbageType} мусор нельзя бросать в {closestBin.acceptedGarbageType} мусорку!");

                StartCoroutine(FlashRed());
            }
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    void LockToBin(GarbageBin bin)
    {
        transform.position = bin.GetDropPosition();
        isLocked = true;

        spriteRenderer.sortingOrder = 2;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        Debug.Log($"✅ {garbageType} мусор выброшен в правильную мусорку!");

        bin.OnGarbageDropped(this);

        lockedGarbageCount++;
        Debug.Log($"Заблокировано мусора: {lockedGarbageCount}/{totalGarbageCount}");

        GarbageGameManager gameManager = FindObjectOfType<GarbageGameManager>();
        if (gameManager != null)
        {
            gameManager.OnGarbageLocked();
        }

        CheckLevelComplete();
    }

    System.Collections.IEnumerator FlashRed()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = originalColor;
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

    void CheckLevelComplete()
    {
        if (lockedGarbageCount >= totalGarbageCount)
        {
            Debug.Log($"🎉 ВЕСЬ МУСОР СОРТИРОВАН! Заблокировано: {lockedGarbageCount}/{totalGarbageCount}");
        }
        else
        {
            Debug.Log($"Осталось мусора: {totalGarbageCount - lockedGarbageCount}");
        }
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    public void ResetGarbage()
    {
        if (isLocked)
        {
            lockedGarbageCount--;
        }

        transform.position = originalPosition;
        isLocked = false;
        isDragging = false;
        spriteRenderer.sortingOrder = 1;
        spriteRenderer.color = Color.white;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = true;
    }

    public static void ResetCounters()
    {
        lockedGarbageCount = 0;
        totalGarbageCount = 0;
    }

    public static void SetTotalGarbageCount(int count)
    {
        totalGarbageCount = count;
    }
}