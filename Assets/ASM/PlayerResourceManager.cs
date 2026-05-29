using UnityEngine;
using System;

public class PlayerResourceManager : MonoBehaviour
{
    public static PlayerResourceManager Instance { get; private set; }

    [Header("Starting Resources")]
    public int gold = 100;
    public int wood = 100;
    public int maxPopulation = 20;

    // Sự kiện phát ra khi tài nguyên thay đổi để HUD tự động cập nhật
    public static event Action OnResourcesChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Không hủy khi chuyển Scene nếu cần (ở đây cùng 1 scene nên giữ đơn giản)
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
