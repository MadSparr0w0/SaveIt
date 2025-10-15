using TMPro;
using UnityEngine;
using System.Collections;

public class WaterDropManager : MonoBehaviour
{
    [Header("Настройки игры")]
    public int totalDrops = 10;
    public int dropsToWin = 8;

    [Header("Ссылки")]
    public GameObject[] waterDrops;
    public GameObject bucket;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI dropsLeftText;

    [Header("Спавн капель")]
    public RectTransform spawnPanel;
    public Transform dropsParent;
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 2f;

    private int dropsCaught = 0;
    private int dropsMissed = 0;
    private int activeDrops = 0;
    private bool gameActive = true;
    private Camera mainCamera;
    private Canvas canvas;
    private int dropsSpawned = 0;
    private GameCompletionManager completionManager;

    void Start()
    {
        mainCamera = Camera.main;
        canvas = FindObjectOfType<Canvas>();


        completionManager = FindObjectOfType<GameCompletionManager>();
        if (completionManager == null)
        {

            GameObject completionObj = new GameObject("GameCompletionManager");
            completionManager = completionObj.AddComponent<GameCompletionManager>();
        }

        if (spawnPanel == null)
        {
            spawnPanel = GameObject.Find("Panel").GetComponent<RectTransform>();
            if (spawnPanel == null)
                Debug.LogError("Не найдена панель для спавна!");
        }

        FindAllWaterDrops();
        StartCoroutine(SpawnDropsRoutine());

        UpdateUI();
        Debug.Log($"Игра началась! Всего капель: {totalDrops}");
    }

    void FindAllWaterDrops()
    {
        waterDrops = new GameObject[totalDrops];
        for (int i = 0; i < totalDrops; i++)
        {
            string dropName = $"{i:00}_0";
            GameObject drop = GameObject.Find(dropName);

            if (drop != null)
            {
                waterDrops[i] = drop;
                waterDrops[i].SetActive(false);

                SetupDropRectTransform(waterDrops[i]);

                WaterDrop dropComponent = waterDrops[i].GetComponent<WaterDrop>();
                if (dropComponent == null)
                {
                    dropComponent = waterDrops[i].AddComponent<WaterDrop>();
                }
                dropComponent.dropManager = this;

                waterDrops[i].tag = "WaterDrop";
            }
            else
            {
                Debug.LogError($"Не найдена капля: {dropName}");
            }
        }

        if (bucket == null)
        {
            bucket = GameObject.Find("buck1");
        }
    }

    void SetupDropRectTransform(GameObject drop)
    {
        RectTransform rectTransform = drop.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = drop.AddComponent<RectTransform>();
        }

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(30, 30);

        if (dropsParent != null)
        {
            drop.transform.SetParent(dropsParent, false);
        }
        else if (spawnPanel != null)
        {
            drop.transform.SetParent(spawnPanel, false);
        }
    }

    System.Collections.IEnumerator SpawnDropsRoutine()
    {
        for (int i = 0; i < totalDrops && gameActive; i++)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            SpawnDrop(i);
            dropsSpawned++;
        }

        StartCoroutine(CheckForGameEnd());
    }

    void SpawnDrop(int index)
    {
        if (index >= waterDrops.Length || waterDrops[index] == null || !gameActive) return;

        Vector2 spawnPosition = GetCorrectSpawnPosition();

        RectTransform rectTransform = waterDrops[index].GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = spawnPosition;
        }
        else
        {
            Vector3 worldPosition = RectTransformToWorldPosition(spawnPosition);
            waterDrops[index].transform.position = worldPosition;
        }

        waterDrops[index].SetActive(true);

        WaterDrop dropComponent = waterDrops[index].GetComponent<WaterDrop>();
        if (dropComponent != null)
        {
            dropComponent.ResetDrop();
        }

        activeDrops++;
        UpdateUI();

        Debug.Log($"Капля {index} запущена на позиции: {spawnPosition}");
    }

    Vector2 GetCorrectSpawnPosition()
    {
        if (spawnPanel == null)
        {
            Debug.LogWarning("Spawn Panel не назначен! Используется резервный спавн.");
            return new Vector2(Random.Range(-200f, 200f), 300f);
        }

        Rect panelRect = spawnPanel.rect;
        float padding = 20f;

        float randomX = Random.Range(-panelRect.width / 2 + padding, panelRect.width / 2 - padding);
        float spawnY = panelRect.height / 2 - padding;

        return new Vector2(randomX, spawnY);
    }

    Vector3 RectTransformToWorldPosition(Vector2 anchoredPosition)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, spawnPanel.TransformPoint(anchoredPosition));
        return mainCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, mainCamera.nearClipPlane));
    }

    public void OnDropCaught()
    {
        if (!gameActive) return;

        dropsCaught++;
        activeDrops--;

        Debug.Log($"Поймано капель: {dropsCaught}");

        UpdateUI();
        CheckGameEnd();
    }

    public void OnDropMissed()
    {
        if (!gameActive) return;

        dropsMissed++;
        activeDrops--;

        Debug.Log($"Упущено капель: {dropsMissed}");

        UpdateUI();
        CheckGameEnd();
    }

    void CheckGameEnd()
    {
        if (dropsCaught >= dropsToWin)
        {
            gameActive = false;
            ShowGameOver(true);
            return;
        }

        int remainingDrops = totalDrops - (dropsCaught + dropsMissed);
        if (dropsCaught + remainingDrops < dropsToWin)
        {
            gameActive = false;
            ShowGameOver(false);
            return;
        }
    }

    System.Collections.IEnumerator CheckForGameEnd()
    {
        yield return new WaitUntil(() => activeDrops == 0);
        yield return new WaitForSeconds(1f);

        if (gameActive && dropsSpawned >= totalDrops)
        {
            bool won = dropsCaught >= dropsToWin;
            ShowGameOver(won);
        }
    }

    void ShowGameOver(bool isWin)
    {
        gameActive = false;

        if (completionManager != null)
        {
            string winMessage = $"ПОБЕДА!\nПоймано: {dropsCaught}/{totalDrops} капель";
            string loseMessage = $"ПРОИГРЫШ!\nПоймано: {dropsCaught}/{totalDrops} капель\nНужно было: {dropsToWin}";

            completionManager.CompleteGame(isWin, winMessage, loseMessage);
        }
        else
        {
            Debug.LogError("GameCompletionManager не найден!");
        }

        Debug.Log($"Игра окончена: {(isWin ? "ПОБЕДА" : "ПРОИГРЫШ")}");
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Поймано: {dropsCaught}";

        if (dropsLeftText != null)
        {
            int dropsLeft = totalDrops - (dropsCaught + dropsMissed);
            dropsLeftText.text = $"Осталось: {dropsLeft}";
        }
    }
}