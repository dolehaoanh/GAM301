using UnityEngine;
using System;

public class PlayerResourceManager : MonoBehaviour
{
    public static PlayerResourceManager Instance { get; private set; }

    [Header("Starting Resources")]
    public int gold = 200;
    public int wood = 200;
    public int maxFood = 50;

    
    public static event Action OnResourcesChanged;

    private void Awake()
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

    public void AddGold(int amount)
    {
        gold += amount;
        OnResourcesChanged?.Invoke();
        Debug.Log($"[PlayerResources] +{amount} GOLD. Total: {gold}");
    }

    public void AddWood(int amount)
    {
        wood += amount;
        OnResourcesChanged?.Invoke();
        Debug.Log($"[PlayerResources] +{amount} WOOD. Total: {wood}");
    }

    public int GetCurrentFoodUsed()
    {
        RTSUnit[] allUnits = FindObjectsByType<RTSUnit>(FindObjectsInactive.Exclude);
        int foodUsed = 0;
        foreach (RTSUnit unit in allUnits)
        {
            
            if (unit != null && unit.transform.position.y > -100f && !unit.isEnemy)
            {
                foodUsed += (unit.unitType == RTSUnitType.Farmer) ? 1 : 2;
            }
        }
        return foodUsed;
    }

    public bool SpendResources(int goldAmount, int woodAmount)
    {
        if (gold >= goldAmount && wood >= woodAmount)
        {
            gold -= goldAmount;
            wood -= woodAmount;
            OnResourcesChanged?.Invoke();
            return true;
        }
        return false;
    }
}
