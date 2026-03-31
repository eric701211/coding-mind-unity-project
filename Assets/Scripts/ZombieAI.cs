using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float attackDistance = 2f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.5f;

    // ADD THIS: A reference to the Animator on the child model
    public Animator animator; 

    private Transform player;
    private PlayerHealth playerHealth;
    private NavMeshAgent agent;
    private float nextAttackTime = 0f;

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
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance)
        {
            agent.isStopped = true;
            animator.SetBool("isChasing", false); // Stop walking animation
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("isChasing", true); // Start walking animation
        }
        else
        {
            agent.isStopped = true; 
            animator.SetBool("isChasing", false); // Stop walking animation
        }
    }

    void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            // Trigger the attack animation
            animator.SetTrigger("Attack"); 
            
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
            nextAttackTime = Time.time + attackCooldown;
        }
    }
}