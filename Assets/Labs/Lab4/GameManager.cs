using UnityEngine;
using UnityEngine.UI; // Required for Slider!

public class GameManager : MonoBehaviour
{
    // Singleton instance so any script can access the GameManager easily
    public static GameManager Instance;

    [Header("Player Stats")]
    public int playerHP = 6;

    [Header("UI References")]
    public Slider hpSlider;          // Drag your HP Slider here
    public GameObject gameOverUI;    // Drag your Game Over Text here

    void Awake()
    {
        // Set up the Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure game time is running normally at the start of the game
        Time.timeScale = 1f;
    }

    void Start()
    {
        // Initialize the Health Bar
        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = 6;
            hpSlider.value = playerHP;
        }

        // Ensure the Game Over UI is hidden at start
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    // Called whenever a monster enters the portal
    public void TakeDamage()
    {
        if (playerHP <= 0) return; // Already game over

        playerHP -= 1;
        Debug.Log($"💥 Gate breached! Lives remaining: {playerHP}");

        // Update the visual health bar
        if (hpSlider != null)
        {
            hpSlider.value = playerHP;
        }

        // Check for Game Over
        if (playerHP <= 0)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        Debug.Log("💀 GAME OVER! Time frozen.");

        // 1. Show the Game Over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 2. Freeze the entire game physics and animations
        Time.timeScale = 0f;
    }
}