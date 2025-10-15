using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Игровая валюта")]
    public int coins = 10000;
    public int greenPoints = 10000;
    public int experience = 0;
    public int playerLevel = 1;

    [Header("UI Elements")]
    public Text coinsText;
    public Text greenPointsText;
    public Text levelText;

    [Header("Сохранение")]
    public bool autoSave = true;
    public float autoSaveInterval = 60f;

    [Header("Настройки новой игры")]
    public bool resetOnFirstLaunch = true;
    public bool unlockStarterAreaOnly = true;

    private float saveTimer = 0f;
    private bool hasResetOnce = false;

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
        CheckFirstLaunchReset();

        LoadGame();
        UpdateUI();
    }

    void CheckFirstLaunchReset()
    {
        int hasReset = PlayerPrefs.GetInt("HasResetOnce", 0);

        if (resetOnFirstLaunch && hasReset == 0)
        {
            Debug.Log("Первый запуск - выполняем сброс прогресса");
            ResetGameProgress();
            PlayerPrefs.SetInt("HasResetOnce", 1);
            PlayerPrefs.Save();
            hasResetOnce = true;
        }
        else
        {
            hasResetOnce = (hasReset == 1);
        }
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
        Debug.Log($"Сохранено объектов: {data.placedObjectsData.Count}");
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
                    if (section != null)
                    {
                        if (sectionData.isUnlocked)
                        {
                            section.Unlock();
                        }
                        else
                        {
                            section.isUnlocked = false;
                            foreach (GameObject cellObj in section.cells)
                            {
                                if (cellObj != null)
                                {
                                    MapCell cell = cellObj.GetComponent<MapCell>();
                                    if (cell != null)
                                    {
                                        cell.Lock();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            LoadPlacedObjects(data.placedObjectsData);

            Debug.Log("Игра загружена! Сохранение от: " + data.saveTime);
            Debug.Log($"Загружено объектов: {data.placedObjectsData.Count}");
        }
        else
        {
            Debug.Log("Сохранение не найдено. Начинаем новую игру.");

            if (unlockStarterAreaOnly && !hasResetOnce)
            {
                UnlockStarterAreaOnly();
            }
        }
    }

    void UnlockStarterAreaOnly()
    {
        if (MapManager.Instance != null && MapManager.Instance.landSections.Count > 0)
        {
            for (int i = 0; i < MapManager.Instance.landSections.Count; i++)
            {
                if (i == 0)
                {
                    MapManager.Instance.landSections[i].Unlock();
                    Debug.Log($"Стартовая зона '{MapManager.Instance.landSections[i].sectionName}' разблокирована");
                }
                else
                {
                    MapManager.Instance.landSections[i].isUnlocked = false;
                    foreach (GameObject cellObj in MapManager.Instance.landSections[i].cells)
                    {
                        if (cellObj != null)
                        {
                            MapCell cell = cellObj.GetComponent<MapCell>();
                            if (cell != null)
                            {
                                cell.Lock();
                            }
                        }
                    }
                    Debug.Log($"Зона '{MapManager.Instance.landSections[i].sectionName}' заблокирована");
                }
            }
        }
    }

    List<ObjectData> GetPlacedObjectsData()
    {
        List<ObjectData> objectsData = new List<ObjectData>();

        PlaceableObject[] placedObjects = FindObjectsOfType<PlaceableObject>();
        Debug.Log($"Найдено объектов на сцене: {placedObjects.Length}");

        foreach (PlaceableObject obj in placedObjects)
        {
            string prefabName = obj.name.Replace("(Clone)", "").Trim();

            ObjectData objData = new ObjectData
            {
                objectName = obj.objectName,
                prefabName = prefabName,
                position = obj.transform.position,
                rotation = obj.transform.eulerAngles,
                coinData = obj.GetCoinData()
            };
            objectsData.Add(objData);

            Debug.Log($"Сохранен объект: {objData.objectName}, префаб: {objData.prefabName}, позиция: {objData.position}");
        }

        return objectsData;
    }

    void LoadPlacedObjects(List<ObjectData> objectsData)
    {
        PlaceableObject[] existingObjects = FindObjectsOfType<PlaceableObject>();
        foreach (PlaceableObject obj in existingObjects)
        {
            if (obj != null)
                Destroy(obj.gameObject);
        }

        Debug.Log($"Загрузка объектов: {objectsData.Count}");

        foreach (ObjectData objData in objectsData)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + objData.prefabName);

            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>(objData.prefabName);
            }

            if (prefab != null)
            {
                GameObject newObj = Instantiate(prefab, objData.position, Quaternion.Euler(objData.rotation));

                PlaceableObject placeable = newObj.GetComponent<PlaceableObject>();
                if (placeable == null)
                {
                    placeable = newObj.AddComponent<PlaceableObject>();
                }

                placeable.objectName = objData.objectName;
                placeable.LoadCoinData(objData.coinData);

                int gridX = Mathf.RoundToInt(objData.position.x);
                int gridZ = Mathf.RoundToInt(objData.position.z);

                for (int x = gridX; x < gridX + placeable.width; x++)
                {
                    for (int z = gridZ; z < gridZ + placeable.height; z++)
                    {
                        MapCell cell = MapManager.Instance.GetCellAtPosition(x, z);
                        if (cell != null)
                        {
                            cell.PlaceObject(newObj);
                        }
                    }
                }

                Debug.Log($"Загружен объект: {objData.objectName} на позиции {objData.position}");
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

    public void ResetGameProgress()
    {
        Debug.Log("Сброс прогресса игры...");

        coins = 1000;
        greenPoints = 500;
        experience = 0;
        playerLevel = 1;

        if (MapManager.Instance != null)
        {
            UnlockStarterAreaOnly();
        }

        PlaceableObject[] allObjects = FindObjectsOfType<PlaceableObject>();
        foreach (PlaceableObject obj in allObjects)
        {
            if (obj != null)
                Destroy(obj.gameObject);
        }

        DeleteSave();

        UpdateUI();
        Debug.Log("Прогресс игры сброшен!");
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

    public void ForceResetProgress()
    {
        ResetGameProgress();
        PlayerPrefs.SetInt("HasResetOnce", 1);
        PlayerPrefs.Save();
    }

    public void CompleteReset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        ResetGameProgress();
        Debug.Log("Полный сброс выполнен!");
    }
}