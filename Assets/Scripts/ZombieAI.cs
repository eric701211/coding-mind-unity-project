using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float attackDistance = 2f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.5f;

    public Animator animator; 

    private Transform player;
    private PlayerHealth playerHealth;
    private NavMeshAgent agent;
    private float nextAttackTime = 0f;
    
    // 1. Add a flag to track if the zombie is dead
    private bool isDead = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        // 2. If the zombie is dead, ignore all the code below so it stops chasing/attacking
        if (isDead) return; 
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance)
        {
            agent.isStopped = true;
            animator.SetBool("isChasing", false);
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("isChasing", true);
        }
        else
        {
            agent.isStopped = true; 
            animator.SetBool("isChasing", false);
        }
    }

    void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack"); 
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // 3. Create a dedicated method to handle death
    public void Die()
    {
        if (isDead) return; // Prevent this from running multiple times
        
        isDead = true; // Flips the flag so Update() stops running
        
        animator.SetTrigger("Die"); // Trigger your death animation
        
        // Turn off the agent ONLY when the zombie actually dies
        if (agent != null) 
        {
            agent.isStopped = true; 
            agent.enabled = false;  
        }

        // Optional but recommended: Disable the collider so the player doesn't trip over the corpse
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
}