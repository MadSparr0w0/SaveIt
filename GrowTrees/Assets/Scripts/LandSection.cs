using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LandSection
{
    public string sectionName;
    public int unlockCost = 100;
    public bool isUnlocked = false;
    public List<GameObject> cells = new List<GameObject>();

    public void Unlock()
    {
        isUnlocked = true;
        foreach (GameObject cellObj in cells)
        {
            if (cellObj != null)
            {
                MapCell cell = cellObj.GetComponent<MapCell>();
                if (cell != null)
                {
                    cell.Unlock();
                }
            }
        }
    }
}