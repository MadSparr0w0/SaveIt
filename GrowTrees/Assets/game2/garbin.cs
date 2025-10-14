using UnityEngine;

public class GarbageBin : MonoBehaviour
{
    [Header("��� ������ ��� ���� �������")]
    public GarbageItem.GarbageType acceptedGarbageType;

    [Header("������� ��� ������ ������")]
    public Vector3 dropOffset = Vector3.zero;

    private int garbageCount = 0;

    void Start()
    {
        // ������������� ���������� ��� ������� �� �����
        AutoDetectBinType();

        Debug.Log($"������� {name} ������. ���������: {acceptedGarbageType}");
    }

    void AutoDetectBinType()
    {
        // ������������� ���������� ��� ������� �� �����
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
            Debug.LogWarning($"�� ������� ���������� ��� ������� ��� {name}. ���������� ��� �� ���������: Plastic");
            acceptedGarbageType = GarbageItem.GarbageType.Plastic;
        }
    }

    // ���������� ����� ����� ��������� ��������
    public void OnGarbageDropped(GarbageItem garbage)
    {
        garbageCount++;
        Debug.Log($"������� {name} ������� {garbageCount} ������ ���� {acceptedGarbageType}");

        // ����� �������� ���� ������ �����, �� ��� ��������� �����
    }

    // ������� ���� ����� ������� ����� (������ �� ������������ ��� ����������������, �� ������� ��� ��������� ��������)
    public Vector3 GetDropPosition()
    {
        return transform.position + dropOffset;
    }

    // ������������ � ���������
    void OnDrawGizmosSelected()
    {
        // ���������� ���� ������������
        Gizmos.color = acceptedGarbageType == GarbageItem.GarbageType.Plastic ?
            Color.blue : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}