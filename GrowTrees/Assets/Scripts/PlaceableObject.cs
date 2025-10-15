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

    private float timer = 0f;
    private bool isReadyForCollection = false;
    private GameObject coinIndicator;

    void Start()
    {
        if (generatesCoins)
        {
            timer = generationTime;
        }
    }

    void Update()
    {
        if (generatesCoins && !isReadyForCollection)
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

        if (coinIndicatorPrefab != null && coinIndicator == null)
        {
            coinIndicator = Instantiate(coinIndicatorPrefab);

            coinIndicator.transform.position = transform.position + new Vector3(0, 3f, 0);

            Debug.Log($"{objectName} готов к сбору!");
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }

    void OnMouseDown()
    {
        if (generatesCoins && isReadyForCollection)
        {
            CollectCoins();
        }
    }

    void CollectCoins()
    {
        GameManager.Instance.AddCoins(coinReward);
        isReadyForCollection = false;
        timer = generationTime;

        if (coinIndicator != null)
        {
            Destroy(coinIndicator);
            coinIndicator = null;
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }

        Debug.Log($"Собрано {coinReward} монет с {objectName}!");
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