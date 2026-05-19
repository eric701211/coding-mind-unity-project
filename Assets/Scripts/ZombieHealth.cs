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

    [Header("Drops")]
    public GameObject healthDropPrefab;
    [Range(0f, 1f)] public float healthDropChance = 1f;
    [Tooltip("How far above the ground the heart's pivot is placed. Increase if your mesh clips into the floor.")]
    public float healthDropGroundOffset = 0.5f;
    [Tooltip("How far above the zombie we start the ground raycast.")]
    public float healthDropRaycastHeight = 2f;
    [Tooltip("Max distance the raycast travels to find the ground.")]
    public float healthDropRaycastDistance = 10f;
    [Tooltip("Layers that count as ground for placing the drop.")]
    public LayerMask healthDropGroundLayers = ~0;
    [Tooltip("How long after death to spawn the drop. Should match the despawn timer.")]
    public float healthDropDelay = 5f;

    [Header("Animation Settings")]
    [Tooltip("Adjust this to slow down or speed up the death animation. 1 is normal, 0.5 is half speed.")]
    public float deathAnimationSpeed = 1f;

    private bool isDead = false;

    // Notice there are now TWO arguments here: amount and hitDirection
    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0f)
        {
            Die();
            return;
        }

        if (animator != null) animator.SetTrigger("Hit");

        StartCoroutine(ApplyKnockback(hitDirection));
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
        StopAllCoroutines();

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.speed = deathAnimationSpeed; // Reset animation speed so it doesn't play too fast/slow
            animator.ResetTrigger("Hit");
            animator.SetBool("isDead", true);
        }
        
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (agent != null) 
        {
            if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }
            agent.enabled = false;
        }
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Stop physics from sliding the zombie
        }
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        
        if (aiScript == null) aiScript = GetComponent<ZombieAI>();
        if (aiScript != null) aiScript.enabled = false;

        Invoke(nameof(TrySpawnHealthDrop), healthDropDelay);
        Destroy(gameObject, healthDropDelay + 0.1f);
    }

    void TrySpawnHealthDrop()
    {
        if (healthDropPrefab == null) return;
        if (Random.value > healthDropChance) return;

        Vector3 spawnPos = GetGroundedSpawnPosition();
        Instantiate(healthDropPrefab, spawnPos, Quaternion.identity);
    }

    Vector3 GetGroundedSpawnPosition()
    {
        Vector3 origin = transform.position + Vector3.up * healthDropRaycastHeight;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, healthDropRaycastDistance, healthDropGroundLayers, QueryTriggerInteraction.Ignore))
        {
            return hit.point + Vector3.up * healthDropGroundOffset;
        }
        return transform.position + Vector3.up * healthDropGroundOffset;
    }
}