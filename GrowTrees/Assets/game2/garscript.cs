using UnityEngine;

public class GarbageGameManager : MonoBehaviour
{
    [Header("Настройки игры")]
    public int totalGarbageCount = 6;

    private int destroyedGarbage = 0;

    void Start()
    {
        GarbageItem[] allGarbage = FindObjectsByType<GarbageItem>(FindObjectsSortMode.None);
        totalGarbageCount = allGarbage.Length;

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
    }
    public void ResetLevel()
    {
        Debug.Log("Для перезапуска перезагрузите сцену");
    }
}