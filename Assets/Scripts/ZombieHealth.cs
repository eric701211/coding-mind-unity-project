using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    public float health = 50f;
    
    [Header("Components")]
    public Animator animator; 
    public NavMeshAgent agent; 
    public ZombieAI aiScript; 

    private bool isDead = false;

    // Notice there are now TWO arguments here: amount and hitDirection
    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        if (isDead) return;

        health -= amount;
        
        if (animator != null) animator.SetTrigger("Hit"); 
        
        StartCoroutine(ApplyKnockback(hitDirection));

        if (health <= 0f)
        {
            Die();
        }
    }

    IEnumerator ApplyKnockback(Vector3 direction)
    {
        float knockbackTime = 0.15f; 
        float knockbackSpeed = 8f;   
        
        if (agent != null && agent.isActiveAndEnabled) agent.isStopped = true;

        float timer = 0;
        while(timer < knockbackTime)
        {
            if (agent != null) agent.Move(direction * knockbackSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null; 
        }

        if (!isDead && agent != null && agent.isActiveAndEnabled) agent.isStopped = false;
    }

    void Die()
    {
        isDead = true;
        if (animator != null) animator.SetBool("isDead", true); 
        
        if (agent != null) agent.enabled = false;
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        
        if (aiScript != null) aiScript.enabled = false;

        Destroy(gameObject, 5f);
    }
}