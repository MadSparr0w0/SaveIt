using UnityEngine;

public class GarbageBin : MonoBehaviour
{
    [Header("Тип мусора для этой мусорки")]
    public GarbageItem.GarbageType acceptedGarbageType;

    [Header("Позиция для сброса мусора")]
    public Vector3 dropOffset = Vector3.zero;

    private int garbageCount = 0;

    void Start()
    {

        AutoDetectBinType();

        Debug.Log($"Мусорка {name} готова. Принимает: {acceptedGarbageType}");
    }

    void AutoDetectBinType()
    {

        if (name.ToLower().Contains("bin1"))
        {
            acceptedGarbageType = GarbageItem.GarbageType.Plastic;
        }
        else if (name.ToLower().Contains("bin2"))
        {
            acceptedGarbageType = GarbageItem.GarbageType.Metal;
        }
        else
        {
            Debug.LogWarning($"Не удалось определить тип мусорки для {name}. Установлен тип по умолчанию: Plastic");
            acceptedGarbageType = GarbageItem.GarbageType.Plastic;
        }
    }


    public void OnGarbageDropped(GarbageItem garbage)
    {
        garbageCount++;
        Debug.Log($"Мусорка {name} приняла {garbageCount} мусора типа {acceptedGarbageType}");


    }

    public Vector3 GetDropPosition()
    {
        return transform.position + dropOffset;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = acceptedGarbageType == GarbageItem.GarbageType.Plastic ?
            Color.blue : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}