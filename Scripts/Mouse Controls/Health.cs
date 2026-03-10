using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Health : MonoBehaviour, IDamagable
{
    public float health = 100;
    public AudioClip damageSound;
    public AudioClip deathSound;

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            source.PlayOneShot(deathSound);
            Destroy(this.gameObject);
        }
        else
        {
            source.PlayOneShot(damageSound);
        }
    }

}
