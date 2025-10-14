using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    [Header("Settings")]
    public float fallSpeed = 2f;
    public float lifeTime = 20f;

    [Header("References")]
    public WaterDropManager dropManager;

    private float timer;
    private bool isActive = true;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        timer = lifeTime;

        if (dropManager == null)
            dropManager = FindObjectOfType<WaterDropManager>();
    }

    void Update()
    {
        if (!isActive) return;

        // Движение вниз через RectTransform
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition -= new Vector2(0, fallSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }

        // Таймер самоуничтожения
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            OnMissed();
        }
    }

    public void ResetDrop()
    {
        isActive = true;
        timer = lifeTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        Debug.Log($"Капля столкнулась с: {other.name} (тег: {other.tag})");

        if (other.CompareTag("Bucket"))
        {
            OnCaught();
        }
    }

    public void OnCaughtByBucket()
    {
        OnCaught();
    }

    void OnCaught()
    {
        if (!isActive) return;

        isActive = false;
        if (dropManager != null)
            dropManager.OnDropCaught();
        else
            Debug.LogError("DropManager не найден!");

        gameObject.SetActive(false);
    }

    void OnMissed()
    {
        if (!isActive) return;

        isActive = false;
        if (dropManager != null)
            dropManager.OnDropMissed();
        else
            Debug.LogError("DropManager не найден!");

        gameObject.SetActive(false);
    }
}