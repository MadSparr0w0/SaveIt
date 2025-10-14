using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GarbageGameManager : MonoBehaviour
{
    [Header("Настройки игры")]
    public int totalGarbageCount = 6;

    [Header("UI элементы")]
    public GameObject gameOverPanel; // Панель окончания игры
    public TextMeshProUGUI gameOverText; // Текст с сообщением
    public Button mainMenuButton; // Кнопка выхода в главное меню

    private int destroyedGarbage = 0;

    void Start()
    {
        GarbageItem[] allGarbage = FindObjectsByType<GarbageItem>(FindObjectsSortMode.None);
        totalGarbageCount = allGarbage.Length;

        // Скрываем панель окончания игры в начале
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Настраиваем кнопку главного меню
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        Debug.Log($"Игра началась! Всего мусора для сортировки: {totalGarbageCount}");
    }

    public void OnGarbageDestroyed()
    {
        destroyedGarbage++;
        Debug.Log($"Уничтожено мусора: {destroyedGarbage}/{totalGarbageCount}");

        if (destroyedGarbage >= totalGarbageCount)
        {
            OnGameComplete();
        }
    }

    void OnGameComplete()
    {
        Debug.Log("🎉 ИГРА ЗАВЕРШЕНА! Весь мусор отсортирован!");

        // Показываем UI с сообщением о завершении игры
        ShowGameOverUI();
    }

    void ShowGameOverUI()
    {
        Debug.Log("Метод ShowGameOverUI() вызван");

        if (gameOverPanel == null)
        {
            Debug.LogError("GameOverPanel не назначена в инспекторе!");
            return;
        }

        Debug.Log($"Панель найдена. Текущее состояние: {gameOverPanel.activeSelf}");

        // Активируем панель
        gameOverPanel.SetActive(true);

        Debug.Log($"Панель активирована. Новое состояние: {gameOverPanel.activeSelf}");

        // Устанавливаем текст
        if (gameOverText != null)
        {
            gameOverText.text = "ИГРА ОКОНЧЕНА";
            Debug.Log("Текст установлен: 'ИГРА ОКОНЧЕНА'");
        }
        else
        {
            Debug.LogError("GameOverText не назначен!");
        }

        // Проверяем кнопку
        if (mainMenuButton == null)
        {
            Debug.LogError("MainMenuButton не назначен!");
        }
        else
        {
            Debug.Log("Кнопка назначена корректно");
        }

        // Принудительно обновляем UI
        Canvas.ForceUpdateCanvases();
    }

    public void GoToMainMenu()
    {
        // Загружаем сцену с главным меню
        // Замените "MainMenu" на имя вашей сцены с главным меню
        SceneManager.LoadScene("MainMenu");
    }

    public void ResetLevel()
    {
        Debug.Log("Для перезапуска перезагрузите сцену");
        // Если хотите перезагрузить текущую сцену:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}