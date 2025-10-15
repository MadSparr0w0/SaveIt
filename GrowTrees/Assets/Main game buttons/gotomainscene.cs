using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMain : MonoBehaviour
{
    [Header("Настройки")]
    public string SceneName = "Main";

    void Start()
    {

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(LoadPuzzleGame);
        }
        else
        {
            Debug.LogError("На объекте нет компонента Button!");
        }
    }

    public void LoadPuzzleGame()
    {
        Debug.Log($"🔄 Загрузка Мейн сцены: {SceneName}");


        SceneManager.LoadScene(SceneName);
    }
}