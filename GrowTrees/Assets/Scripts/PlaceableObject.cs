using UnityEngine;
using UnityEngine.UI;

public class PlaceableObject : MonoBehaviour
{
    [Header("Настройки объекта")]
    public string objectName;
    public int width = 1;
    public int height = 1;
    public int cost;

    [Header("Заработок монет")]
    public bool generatesCoins = false;
    public int coinReward = 5;
    public float generationTime = 60f;

    [Header("Индикатор монет")]
    public GameObject coinIndicatorPrefab;
    public Vector3 indicatorOffset = new Vector3(0, 3f, 0);

    [HideInInspector]
    public float timer = 0f;
    [HideInInspector]
    public bool isReadyForCollection = false;
    [HideInInspector]
    public GameObject coinIndicator;

    private Material originalMaterial;
    private Color originalColor;
    private bool isBeingEdited = false;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
            originalColor = renderer.material.color;
        }

        if (generatesCoins)
        {
            timer = generationTime;
        }
    }

    void Update()
    {
        if (generatesCoins && !isReadyForCollection && !isBeingEdited)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ReadyForCollection();
            }
        }
    }

    void ReadyForCollection()
    {
        isReadyForCollection = true;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }

    void OnMouseDown()
    {
        if (ShopManager.Instance != null && ShopManager.Instance.isShopOpen)
            return;

        if (ObjectEditManager.Instance != null && ObjectEditManager.Instance.IsEditing())
            return;

        if (generatesCoins && isReadyForCollection)
        {
            CollectCoins();
        }
        else
        {
            if (ObjectEditManager.Instance != null)
            {
                isBeingEdited = true;
                ObjectEditManager.Instance.SelectObject(this);
            }
        }
    }

    public void CollectCoins()
    {
        if (!isReadyForCollection) return;

        GameManager.Instance.AddCoins(coinReward);
        isReadyForCollection = false;
        timer = generationTime;
        isBeingEdited = false;

        if (coinIndicator != null)
        {
            Destroy(coinIndicator);
            coinIndicator = null;
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && originalMaterial != null)
        {
            renderer.material = originalMaterial;
            renderer.material.color = originalColor;
        }

        Debug.Log($"Собрано {coinReward} монет с {objectName}!");
    }

    public void OnEditFinished()
    {
        isBeingEdited = false;
    }

    public CoinData GetCoinData()
    {
        return new CoinData
        {
            timer = timer,
            isReadyForCollection = isReadyForCollection
        };
    }

    public void LoadCoinData(CoinData data)
    {
        timer = data.timer;
        isReadyForCollection = data.isReadyForCollection;

        if (isReadyForCollection)
        {
            ReadyForCollection();
        }
    }
}