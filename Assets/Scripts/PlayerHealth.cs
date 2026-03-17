using UnityEngine;
using UnityEngine.UI; // Add this

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public Text healthText; // Drag your HealthText UI here

    void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        UpdateUI();

        if (health <= 0f)
        {
            if (healthText != null) healthText.text = "HP: 0 (DEAD)";
            Debug.Log("Player Died!");
        }
    }

    void UpdateUI()
    {
        if (healthText != null) healthText.text = "HP: " + health;
    }
}