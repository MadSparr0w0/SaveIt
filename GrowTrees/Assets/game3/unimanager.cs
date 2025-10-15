using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameCompletionManager : MonoBehaviour
{
    [Header("UI элементы")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button mainMenuButton;

    [Header("Настройки")]
    public string mainMenuSceneName = "Main";

    private bool gameCompleted = false;

    void Start()
    {
        // Скрываем панель окончания игры в начале
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Настраиваем кнопки
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        Debug.Log("GameCompletionManager инициализирован");
    }

    // Метод для завершения игры с кастомным сообщением
    public void CompleteGame(bool isWin, string winMessage, string loseMessage)
    {
        if (gameCompleted) return;

        gameCompleted = true;
        Debug.Log("✅ Игра завершена!");

        StartCoroutine(ShowGameOverCoroutine(isWin, winMessage, loseMessage));
    }

    // Перегруженный метод для простого использования
    public void CompleteGame(bool isWin)
    {
        string winMessage = "ПОБЕДА!";
        string loseMessage = "ПРОИГРЫШ!";
        CompleteGame(isWin, winMessage, loseMessage);
    }

    IEnumerator ShowGameOverCoroutine(bool isWin, string winMessage, string loseMessage)
    {
        Debug.Log("🔄 Запуск корутины показа экрана завершения...");
        yield return null;

        // Показываем UI
        ShowGameOverUI(isWin, winMessage, loseMessage);

        Debug.Log("✅ Корутина завершена");
    }

    void ShowGameOverUI(bool isWin, string winMessage, string loseMessage)
    {
        // Если панель назначена в инспекторе - используем ее
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Устанавливаем текст в зависимости от результата
            if (gameOverText != null)
            {
                gameOverText.text = isWin ? winMessage : loseMessage;
                gameOverText.color = isWin ? Color.green : Color.red;
            }

            Debug.Log("✅ Панель завершения игры активирована");
        }
        else
        {
            // Если нет - создаем UI через код
            Debug.Log("🛠️ Создание UI через код...");
            CreateGameOverUI(isWin, winMessage, loseMessage);
        }
    }

    void CreateGameOverUI(bool isWin, string winMessage, string loseMessage)
    {
        // Создаем Canvas
        GameObject canvasObj = new GameObject("GameOverCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Создаем панель
        gameOverPanel = new GameObject("Panel");
        gameOverPanel.transform.SetParent(canvasObj.transform);
        RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = gameOverPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        // Создаем текст
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(gameOverPanel.transform);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.5f);
        textRect.anchorMax = new Vector2(0.9f, 0.8f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        gameOverText = textObj.AddComponent<TextMeshProUGUI>();
        gameOverText.text = isWin ? winMessage : loseMessage;
        gameOverText.color = isWin ? Color.green : Color.red;
        gameOverText.alignment = TextAlignmentOptions.Center;
        gameOverText.fontSize = 72;
        gameOverText.enableAutoSizing = true;
        gameOverText.fontSizeMin = 24;
        gameOverText.fontSizeMax = 72;

        // Создаем кнопку главного меню
        GameObject menuButtonObj = new GameObject("MenuButton");
        menuButtonObj.transform.SetParent(gameOverPanel.transform);
        RectTransform menuButtonRect = menuButtonObj.AddComponent<RectTransform>();
        menuButtonRect.anchorMin = new Vector2(0.3f, 0.2f);
        menuButtonRect.anchorMax = new Vector2(0.7f, 0.35f);
        menuButtonRect.offsetMin = Vector2.zero;
        menuButtonRect.offsetMax = Vector2.zero;

        Image menuButtonImage = menuButtonObj.AddComponent<Image>();
        menuButtonImage.color = Color.gray;

        mainMenuButton = menuButtonObj.AddComponent<Button>();
        mainMenuButton.onClick.AddListener(GoToMainMenu);

        // Текст кнопки главного меню
        GameObject menuButtonTextObj = new GameObject("ButtonText");
        menuButtonTextObj.transform.SetParent(menuButtonObj.transform);
        RectTransform menuButtonTextRect = menuButtonTextObj.AddComponent<RectTransform>();
        menuButtonTextRect.anchorMin = Vector2.zero;
        menuButtonTextRect.anchorMax = Vector2.one;
        menuButtonTextRect.offsetMin = Vector2.zero;
        menuButtonTextRect.offsetMax = Vector2.zero;

        TextMeshProUGUI menuButtonTmpText = menuButtonTextObj.AddComponent<TextMeshProUGUI>();
        menuButtonTmpText.text = "Главное меню";
        menuButtonTmpText.color = Color.black;
        menuButtonTmpText.alignment = TextAlignmentOptions.Center;
        menuButtonTmpText.fontSize = 32;

        Debug.Log("🎯 UI ЗАВЕРШЕНИЯ ИГРЫ СОЗДАН!");
    }

    public void GoToMainMenu()
    {
        Debug.Log("🔄 Загрузка главного меню...");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}