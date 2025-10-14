using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [Header("Секции карты")]
    public List<LandSection> landSections = new List<LandSection>();

    public static MapManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public MapCell GetCellAtPosition(int x, int y)
    {
        foreach (LandSection section in landSections)
        {
            foreach (GameObject cellObj in section.cells)
            {
                if (cellObj != null)
                {
                    MapCell cell = cellObj.GetComponent<MapCell>();
                    if (cell != null && cell.gridX == x && cell.gridY == y)
                        return cell;
                }
            }
        }
        return null;
    }
}