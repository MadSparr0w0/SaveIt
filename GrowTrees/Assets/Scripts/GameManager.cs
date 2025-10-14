using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Игровая валюта")]
    public int coins = 1000;
    public int greenPoints = 500;
    public int experience = 0;
    public int playerLevel = 1;

    [Header("UI Elements")]
    public Text coinsText;
    public Text greenPointsText;
    public Text levelText;

    [Header("Сохранение")]
    public bool autoSave = true;
    public float autoSaveInterval = 60f; 

    private float saveTimer = 0f;

    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadGame();
        UpdateUI();
    }

    void Update()
    {
        if (autoSave)
        {
            saveTimer += Time.deltaTime;
            if (saveTimer >= autoSaveInterval)
            {
                SaveGame();
                saveTimer = 0f;
            }
        }
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        data.coins = coins;
        data.greenPoints = greenPoints;
        data.experience = experience;
        data.playerLevel = playerLevel;
        data.saveTime = System.DateTime.Now;

        if (MapManager.Instance != null)
        {
            foreach (LandSection section in MapManager.Instance.landSections)
            {
                SectionData sectionData = new SectionData
                {
                    sectionName = section.sectionName,
                    isUnlocked = section.isUnlocked
                };
                data.sectionsData.Add(sectionData);
            }
        }

        data.placedObjectsData = GetPlacedObjectsData();

        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString("GameSave", json);
        PlayerPrefs.Save();

        Debug.Log("Игра сохранена! " + System.DateTime.Now.ToString());
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("GameSave"))
        {
            string json = PlayerPrefs.GetString("GameSave");
            GameData data = JsonUtility.FromJson<GameData>(json);

            coins = data.coins;
            greenPoints = data.greenPoints;
            experience = data.experience;
            playerLevel = data.playerLevel;

            if (MapManager.Instance != null)
            {
                foreach (SectionData sectionData in data.sectionsData)
                {
                    LandSection section = MapManager.Instance.landSections.Find(s => s.sectionName == sectionData.sectionName);
                    if (section != null && sectionData.isUnlocked)
                    {
                        section.Unlock();
                    }
                }
            }

            LoadPlacedObjects(data.placedObjectsData);

            Debug.Log("Игра загружена! Сохранение от: " + data.saveTime);
        }
        else
        {
            Debug.Log("Сохранение не найдено. Начинаем новую игру.");
        }
    }

    List<ObjectData> GetPlacedObjectsData()
    {
        List<ObjectData> objectsData = new List<ObjectData>();

        PlaceableObject[] placedObjects = FindObjectsOfType<PlaceableObject>();
        foreach (PlaceableObject obj in placedObjects)
        {
            ObjectData objData = new ObjectData
            {
                objectName = obj.objectName,
                prefabName = obj.name.Replace("(Clone)", ""),
                position = obj.transform.position,
                rotation = obj.transform.eulerAngles,
                coinData = obj.GetCoinData()
            };
            objectsData.Add(objData);
        }

        return objectsData;
    }

    void LoadPlacedObjects(List<ObjectData> objectsData)
    {
        PlaceableObject[] existingObjects = FindObjectsOfType<PlaceableObject>();
        foreach (PlaceableObject obj in existingObjects)
        {
            Destroy(obj.gameObject);
        }

        foreach (ObjectData objData in objectsData)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + objData.prefabName);
            if (prefab != null)
            {
                GameObject newObj = Instantiate(prefab, objData.position, Quaternion.Euler(objData.rotation));
                PlaceableObject placeable = newObj.GetComponent<PlaceableObject>();

                if (placeable != null)
                {
                    placeable.LoadCoinData(objData.coinData);
                }

                MapCell cell = MapManager.Instance.GetCellAtPosition(
                    Mathf.RoundToInt(objData.position.x),
                    Mathf.RoundToInt(objData.position.z)
                );
                if (cell != null)
                {
                    cell.PlaceObject(newObj);
                }
            }
            else
            {
                Debug.LogWarning($"Не найден префаб: {objData.prefabName}");
            }
        }
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    public bool SpendGreenPoints(int amount)
    {
        if (greenPoints >= amount)
        {
            greenPoints -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddGreenPoints(int amount)
    {
        greenPoints += amount;
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinsText) coinsText.text = $"Coins: {coins}";
        if (greenPointsText) greenPointsText.text = $"Green Points: {greenPoints}";
        if (levelText) levelText.text = $"Level: {playerLevel}";
    }

    public void QuickSave()
    {
        SaveGame();
    }

    public void QuickLoad()
    {
        LoadGame();
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("GameSave");
        PlayerPrefs.Save();
        Debug.Log("Сохранение удалено!");
    }
}