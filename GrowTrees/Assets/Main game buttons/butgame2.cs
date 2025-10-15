using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame2 : MonoBehaviour
{
    [Header("Настройки")]
    public string SceneName = "minigame2 trash"; // Имя сцены с пазлами

    void Start()
    {
        // Находим кнопку и назначаем обработчик клика
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(LoadGame);
        }
        else
        {
            Debug.LogError("На объекте нет компонента Button!");
        }
    }

    public void LoadGame()
    {
        Debug.Log($"🔄 Загрузка сцены с мусором: {SceneName}");

        // Загружаем сцену с пазлами
        SceneManager.LoadScene(SceneName);
    }
}