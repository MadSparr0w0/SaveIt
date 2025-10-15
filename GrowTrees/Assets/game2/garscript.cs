using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GarbageGameManager : MonoBehaviour
{
    [Header("UI элементы")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button mainMenuButton;

    private int totalGarbageCount = 0;
    private int lockedGarbageCount = 0;
    private bool gameCompleted = false;

    void Start()
    {
        GarbageItem[] allGarbage = FindObjectsOfType<GarbageItem>(true);
        totalGarbageCount = allGarbage.Length;

        GarbageItem.ResetCounters();
        GarbageItem.SetTotalGarbageCount(totalGarbageCount);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        Debug.Log($"GarbageGameManager инициализирован. Всего мусора: {totalGarbageCount}");

        foreach (GarbageItem garbage in allGarbage)
        {
            Debug.Log($"Найден мусор: {garbage.name}, активен: {garbage.gameObject.activeInHierarchy}");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("=== ТЕСТОВЫЙ ВЫЗОВ ЗАВЕРШЕНИЯ ИГРЫ ===");
            Debug.Log($"Всего мусора: {totalGarbageCount}, Заблокировано: {lockedGarbageCount}");
            OnLevelCompleted();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowGarbageInfo();
        }
    }

    public void OnGarbageLocked()
    {
        lockedGarbageCount++;
        Debug.Log($"Заблокирован мусор. Всего: {lockedGarbageCount}/{totalGarbageCount}");

        if (lockedGarbageCount >= totalGarbageCount && !gameCompleted)
        {
            gameCompleted = true;
            Debug.Log("✅ Уровень завершен! Весь мусор отсортирован!");
            StartCoroutine(ShowGameOverCoroutine());
        }
    }

    void ShowGarbageInfo()
    {
        GarbageItem[] allGarbage = FindObjectsOfType<GarbageItem>(true);
        int activeCount = 0;
        int lockedCount = 0;

        foreach (GarbageItem garbage in allGarbage)
        {
            if (garbage.gameObject.activeInHierarchy) activeCount++;
            if (garbage.IsLocked()) lockedCount++;
        }

        Debug.Log($"ИНФО: Всего объектов: {allGarbage.Length}, Активных: {activeCount}, Заблокировано: {lockedCount}");
    }

    IEnumerator ShowGameOverCoroutine()
    {
        Debug.Log("🔄 Запуск корутины показа экрана завершения...");
        yield return null;

        ShowGameOverUI();

        Debug.Log("✅ Корутина завершена");
    }

    void ShowGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("✅ Панель завершения игры активирована");
        }
        else
        {
            Debug.Log("🛠️ Создание UI через код...");
            CreateGameOverUI();
        }
    }

    void CreateGameOverUI()
    {
        GameObject canvasObj = new GameObject("GameOverCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        gameOverPanel = new GameObject("Panel");
        gameOverPanel.transform.SetParent(canvasObj.transform);
        RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = gameOverPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(gameOverPanel.transform);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.2f, 0.6f);
        textRect.anchorMax = new Vector2(0.8f, 0.8f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        gameOverText = textObj.AddComponent<TextMeshProUGUI>();
        gameOverText.text = "ИГРА ОКОНЧЕНА!";
        gameOverText.color = Color.white;
        gameOverText.alignment = TextAlignmentOptions.Center;
        gameOverText.fontSize = 72;

        GameObject buttonObj = new GameObject("Button");
        buttonObj.transform.SetParent(gameOverPanel.transform);
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.3f, 0.3f);
        buttonRect.anchorMax = new Vector2(0.7f, 0.4f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.gray;

        mainMenuButton = buttonObj.AddComponent<Button>();
        mainMenuButton.onClick.AddListener(GoToMainMenu);

        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform);
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        TextMeshProUGUI buttonTmpText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonTmpText.text = "Главное меню";
        buttonTmpText.color = Color.black;
        buttonTmpText.alignment = TextAlignmentOptions.Center;
        buttonTmpText.fontSize = 24;

        Debug.Log("🎯 UI ЗАВЕРШЕНИЯ ИГРЫ СОЗДАН!");
    }

    public void OnLevelCompleted()
    {
        if (gameCompleted) return;

        gameCompleted = true;
        Debug.Log("✅ Уровень завершен! Весь мусор отсортирован!");
        StartCoroutine(ShowGameOverCoroutine());
    }

    public void GoToMainMenu()
    {
        Debug.Log("🔄 Загрузка главного меню...");
        SceneManager.LoadScene("Main"); 
    }

    public void ResetLevel()
    {
        Debug.Log("🔄 Перезагрузка уровня...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}