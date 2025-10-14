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
        // Автоматически определяем тип мусорки по имени
        AutoDetectBinType();

        Debug.Log($"Мусорка {name} готова. Принимает: {acceptedGarbageType}");
    }

    void AutoDetectBinType()
    {
        // Автоматически определяем тип мусорки по имени
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

    // Вызывается когда мусор правильно выброшен
    public void OnGarbageDropped(GarbageItem garbage)
    {
        garbageCount++;
        Debug.Log($"Мусорка {name} приняла {garbageCount} мусора типа {acceptedGarbageType}");

        // Можно добавить звук успеха здесь, но без изменения цвета
    }

    // Позиция куда будет помещен мусор (теперь не используется для позиционирования, но оставим для возможных эффектов)
    public Vector3 GetDropPosition()
    {
        return transform.position + dropOffset;
    }

    // Визуализация в редакторе
    void OnDrawGizmosSelected()
    {
        // Показываем зону притягивания
        Gizmos.color = acceptedGarbageType == GarbageItem.GarbageType.Plastic ?
            Color.blue : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}