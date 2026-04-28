using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float attackDistance = 2f;
    public float attackDamage = 15f;
    public float attackCooldown = 1.5f;
    public float faceTargetTurnSpeed = 8f;
    public GameObject healthDropPrefab;

    [Header("Movement")]
    [Tooltip("How fast the zombie chases the player (NavMeshAgent speed).")]
    public float chaseSpeed = 3.5f;
    [Tooltip("The movement speed the walk animation was authored at. The animator playback speed is scaled so it visually matches the agent's actual speed. For most mixamo-style walk clips ~1.3 works well; tune until the feet don't slide.")]
    public float walkAnimationReferenceSpeed = 1.3f;
    [Tooltip("Clamp how slow/fast the walk animation can play. Prevents jittery extremes.")]
    public float minAnimationSpeed = 0.5f;
    public float maxAnimationSpeed = 3f;

    [Header("Attack Hit")]
    [Tooltip("Time (seconds) from start of attack animation to the impact moment.")]
    public float attackHitDelay = 0.5f;
    [Tooltip("Extra distance beyond attackDistance still allowed to land the hit.")]
    public float attackHitRange = 0.5f;
    [Tooltip("Cone in front of zombie (degrees) the player must be within to get hit.")]
    public float attackHitAngle = 90f;

    public Animator animator; 

    private Transform player;
    private PlayerHealth playerHealth;
    private NavMeshAgent agent;
    private float nextAttackTime = 0f;
    private static readonly int AttackStateHash = Animator.StringToHash("ZombieRig|Attack1");
    
    // 1. Add a flag to track if the zombie is dead
    private bool isDead = false; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = chaseSpeed;

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
            agent.velocity = Vector3.zero;
            animator.SetBool("isChasing", false);
            FacePlayer();
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            animator.SetBool("isChasing", true);
        }
        else
        {
            agent.isStopped = true; 
            animator.SetBool("isChasing", false);
        }

        SyncWalkAnimationSpeed();
    }

    void SyncWalkAnimationSpeed()
    {
        if (animator == null || agent == null) return;

        if (!animator.GetBool("isChasing") || walkAnimationReferenceSpeed <= 0f)
        {
            animator.speed = 1f;
            return;
        }

        float agentSpeed = agent.velocity.magnitude;
        float ratio = agentSpeed / walkAnimationReferenceSpeed;
        animator.speed = Mathf.Clamp(ratio, minAnimationSpeed, maxAnimationSpeed);
    }

    void AttackPlayer()
    {
        if (Time.time < nextAttackTime) return;

        AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(0);
        if (current.shortNameHash == AttackStateHash || animator.IsInTransition(0)) return;

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

        nextAttackTime = Time.time + attackCooldown;
        StartCoroutine(DealDamageAfterDelay());
    }

    IEnumerator DealDamageAfterDelay()
    {
        yield return new WaitForSeconds(attackHitDelay);

        if (isDead || player == null || playerHealth == null) yield break;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        float dist = toPlayer.magnitude;
        if (dist > attackDistance + attackHitRange) yield break;

        if (dist > 0.0001f)
        {
            float angle = Vector3.Angle(transform.forward, toPlayer);
            if (angle > attackHitAngle * 0.5f) yield break;
        }

        playerHealth.TakeDamage(attackDamage, transform.position);
    }

    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * faceTargetTurnSpeed);
    }

    // 3. Create a dedicated method to handle death
    public void Die()
    {
        if (isDead) return; // Prevent this from running multiple times
        
        isDead = true; // Flips the flag so Update() stops running

        if (animator != null) animator.speed = 1f;
        animator.SetTrigger("Die"); // Trigger your death animation
        
        Instantiate(healthDropPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
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