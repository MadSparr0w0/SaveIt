using UnityEngine;

public class GarbageAutoManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== АВТОМАТИЧЕСКАЯ ПРОВЕРКА СИСТЕМЫ ===");

        GarbageItem[] allGarbage = FindObjectsByType<GarbageItem>(FindObjectsSortMode.None);
        Debug.Log($"Найдено мусора: {allGarbage.Length}");

        foreach (GarbageItem garbage in allGarbage)
        {
            string type = garbage.name.ToLower().Contains("met") ? "METAL" : "PLASTIC";
            Debug.Log($"Мусор: {garbage.name} -> Автоопределен как: {type}");
        }

        GarbageBin[] allBins = FindObjectsByType<GarbageBin>(FindObjectsSortMode.None);
        Debug.Log($"Найдено мусорок: {allBins.Length}");

        foreach (GarbageBin bin in allBins)
        {
            string expectedType = bin.name.ToLower().Contains("bin1") ? "PLASTIC" :
                                 bin.name.ToLower().Contains("bin2") ? "METAL" : "НЕОПРЕДЕЛЕН";
            Debug.Log($"Мусорка: {bin.name} -> Автоопределена как: {expectedType}");
        }

        Debug.Log("=== ПРОВЕРКА ЗАВЕРШЕНА ===");
    }
}