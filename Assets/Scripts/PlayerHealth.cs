using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public Text healthText;

    [Header("Damage Feedback")]
    public Image damageOverlay;
    public Color damageColor = new Color(1f, 0f, 0f, 0.45f);
    public float flashFadeSpeed = 2f;

    [Header("Knockback")]
    public Rigidbody playerRigidbody;
    public float knockbackForce = 6f;
    public float knockbackUpward = 0.2f;

    private Coroutine flashRoutine;

    void Start()
    {
        if (playerRigidbody == null) playerRigidbody = GetComponent<Rigidbody>();

        if (damageOverlay != null)
        {
            Color c = damageOverlay.color;
            c.a = 0f;
            damageOverlay.color = c;
        }

        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, transform.position);
    }

    public void TakeDamage(float amount, Vector3 sourcePosition)
    {
        health -= amount;
        UpdateUI();

        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashDamageOverlay());

        ApplyKnockback(sourcePosition);

        if (health <= 0f)
        {
            if (healthText != null) healthText.text = "HP: 0 (DEAD)";
            Debug.Log("Player Died!");
        }
    }

    public void Heal(float amount)
    {
        if (health <= 0f) return;
        health = Mathf.Min(health + amount, maxHealth);
        UpdateUI();
    }

    void ApplyKnockback(Vector3 sourcePosition)
    {
        if (playerRigidbody == null) return;

        Vector3 dir = transform.position - sourcePosition;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = -transform.forward;
        dir.Normalize();
        dir += Vector3.up * knockbackUpward;

        playerRigidbody.AddForce(dir * knockbackForce, ForceMode.Impulse);
    }

    IEnumerator FlashDamageOverlay()
    {
        if (damageOverlay == null) yield break;

        damageOverlay.color = damageColor;

        while (damageOverlay.color.a > 0f)
        {
            Color c = damageOverlay.color;
            c.a = Mathf.Max(0f, c.a - Time.deltaTime * flashFadeSpeed);
            damageOverlay.color = c;
            yield return null;
        }
    }

    void UpdateUI()
    {
        if (healthText != null) healthText.text = "HP: " + health;
    }
}
