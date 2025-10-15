using UnityEngine;

public class GarbageAutoManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== �������������� �������� ������� ===");

        GarbageItem[] allGarbage = FindObjectsByType<GarbageItem>(FindObjectsSortMode.None);
        Debug.Log($"������� ������: {allGarbage.Length}");

        foreach (GarbageItem garbage in allGarbage)
        {
            string type = garbage.name.ToLower().Contains("met") ? "METAL" : "PLASTIC";
            Debug.Log($"�����: {garbage.name} -> ������������� ���: {type}");
        }

        GarbageBin[] allBins = FindObjectsByType<GarbageBin>(FindObjectsSortMode.None);
        Debug.Log($"������� �������: {allBins.Length}");

        foreach (GarbageBin bin in allBins)
        {
            string expectedType = bin.name.ToLower().Contains("bin1") ? "PLASTIC" :
                                 bin.name.ToLower().Contains("bin2") ? "METAL" : "�����������";
            Debug.Log($"�������: {bin.name} -> �������������� ���: {expectedType}");
        }

        Debug.Log("=== �������� ��������� ===");
    }
}