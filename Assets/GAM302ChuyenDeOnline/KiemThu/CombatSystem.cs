using UnityEngine;

// Lop quan ly he thong chien dau va tinh toan sat thuong trong game
public class CombatSystem : MonoBehaviour
{
    // Ham tinh toan sat thuong dua tren sat thuong co ban va he so nhan
    public int CalculateDamage(int baseDamage, float multiplier)
    {
        return Mathf.FloorToInt(baseDamage * multiplier);
    }
}
