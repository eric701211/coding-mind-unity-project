using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float detectionRadius = 15f; // How close the player needs to be to trigger a chase
    public float attackDistance = 2f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.5f;

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

        // Calculate distance once per frame
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // STATE 1: Player is close enough to attack
        if (distanceToPlayer <= attackDistance)
        {
            agent.isStopped = true; // Stop moving
            AttackPlayer();
        }
        // STATE 2: Player is outside attack range, but inside detection range (Chase)
        else if (distanceToPlayer <= detectionRadius)
        {
            agent.isStopped = false; // Resume moving
            agent.SetDestination(player.position);
        }
        // STATE 3: Player is too far away (Idle)
        else
        {
            // Stop moving if the player escapes the detection radius
            agent.isStopped = true; 
        }
    }

    void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // This draws visual debugging spheres in the Unity Editor when you click on the zombie
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere for the detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw a red sphere for the attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}