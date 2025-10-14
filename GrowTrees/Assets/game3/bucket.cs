using UnityEngine;

public class BucketController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 5f;
    public float xLimit = 8f; // Ограничение по горизонтали

    private Camera mainCamera;
    private Vector3 dragOffset;
    private bool isDragging = false;

    void Start()
    {
        mainCamera = Camera.main;

        // Автоматически добавляем коллайдер если нет
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorldPos;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 targetPosition = mouseWorldPos + dragOffset;

        // Ограничиваем движение по горизонтали
        targetPosition.x = Mathf.Clamp(targetPosition.x, -xLimit, xLimit);
        targetPosition.y = transform.position.y; // Сохраняем Y позицию

        transform.position = targetPosition;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        // Альтернативное управление клавишами (для тестирования)
        HandleKeyboardInput();
    }

    void HandleKeyboardInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0)
        {
            Vector3 newPosition = transform.position + new Vector3(horizontal * moveSpeed * Time.deltaTime, 0, 0);
            newPosition.x = Mathf.Clamp(newPosition.x, -xLimit, xLimit);
            transform.position = newPosition;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    // Обработка столкновения с каплями
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("WaterDrop"))
        {
            WaterDrop drop = collision.GetComponent<WaterDrop>();
            if (drop != null)
            {
                drop.OnCaughtByBucket();
            }
        }
    }
}