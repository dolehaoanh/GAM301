using UnityEngine;

public class MonsterHP : MonoBehaviour
{
    [Header("Health Settings")]
    public int currentHP;

    private int[] hpOptions = { 3, 9, 81 };

    void Start()
    {
        // 1. Pick a random HP value from our options: 3, 9, or 81
        int randomIndex = Random.Range(0, hpOptions.Length);
        currentHP = hpOptions[randomIndex];

        // 2. Change color based on the selected HP
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            if (currentHP == 3)
            {
                rend.material.color = Color.green; // 3 HP = Green
            }
            else if (currentHP == 9)
            {
                rend.material.color = Color.blue;  // 9 HP = Blue
            }
            else if (currentHP == 81)
            {
                rend.material.color = Color.red;   // 81 HP = Red
            }
        }
    }

    // Called by the bullet script to deal damage
    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current HP: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log("💀 Monster defeated!");
            Destroy(gameObject);
        }
    }
}