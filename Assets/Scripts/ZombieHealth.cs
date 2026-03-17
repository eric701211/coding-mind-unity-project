using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Zombie eliminated!");
        Destroy(gameObject);
    }
}