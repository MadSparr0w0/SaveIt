using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    [Header("������� ������")]
    public int coins;
    public int greenPoints;
    public int playerLevel;
    public int experience;

    [Header("�����")]
    public List<SectionData> sectionsData = new List<SectionData>();
    public List<ObjectData> placedObjectsData = new List<ObjectData>();

    [Header("���������")]
    public string saveVersion = "1.0";
    public System.DateTime saveTime;
}

[System.Serializable]
public class SectionData
{
    public string sectionName;
    public bool isUnlocked;
}

[System.Serializable]
public class ObjectData
{
    public string objectName;
    public string prefabName;
    public Vector3 position;
    public Vector3 rotation;
    public CoinData coinData;
}