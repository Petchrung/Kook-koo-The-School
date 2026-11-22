
using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Detection")]
    public float detectDistance = 15f;
    public float loseDistance = 20f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Attack")]
    public float attackDistance = 2f;
    public int attackDamage = 1;
    public float attackCooldown = 1.5f;

    [Header("Patrol")]
    public Transform[] patrolPoints;

    [Header("Animation")]
    public Animator animator;

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;

    private int currentPoint = 0;
    private bool chasing = false;
    private float waitTimer = 0f;
    private float nextAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("Ghost has no NavMeshAgent!");
            enabled = false;
            return;
        }

        // ค้นหา Player
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");

            if (found != null)
                player = found.transform;
        }

        if (player == null)
        {
            Debug.LogError("Cannot find Player! Check Player Tag.");
            enabled = false;
            return;
        }

        playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = player.GetComponentInParent<PlayerHealth>();

        agent.speed = patrolSpeed;
        agent.updateRotation = true;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Ghost is not on NavMesh! Move Ghost onto blue area.");
            enabled = false;
            return;
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
            GoToNextPoint();

        Debug.Log("Ghost AI Ready!");
    }

    void Update()
    {
        if (player == null || agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        // คำนวณระยะแนวราบ
        Vector3 ghostPos = transform.position;
        Vector3 playerPos = player.position;

        ghostPos.y = 0;
        playerPos.y = 0;

        float distance = Vector3.Distance(ghostPos, playerPos);

        // ตรวจจับผู้เล่น
        if (!chasing && distance <= detectDistance)
        {
            chasing = true;
            Debug.Log("Ghost detected Player!");
        }

        // ไล่ตาม
        if (chasing)
        {
            if (distance > loseDistance)
            {
                chasing = false;
                agent.isStopped = false;
                agent.speed = patrolSpeed;

                if (patrolPoints != null && patrolPoints.Length > 0)
                    GoToNextPoint();
            }
            else
            {
                ChasePlayer(distance);
            }
        }
        else
        {
            Patrol();
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    void ChasePlayer(float distance)
    {
        agent.speed = chaseSpeed;

        if (distance <= attackDistance)
        {
            agent.isStopped = true;
            LookAtPlayer();

            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    void AttackPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Ghost attacked Player!");
        }
        else
        {
            Debug.LogWarning("PlayerHealth not found!");
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                target,
                Time.deltaTime * 8f
            );
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        agent.speed = patrolSpeed;
        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= 2f)
            {
                GoToNextPoint();
                waitTimer = 0f;
            }
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        agent.isStopped = false;

        agent.SetDestination(patrolPoints[currentPoint].position);

        currentPoint++;

        if (currentPoint >= patrolPoints.Length)
            currentPoint = 0;
    }
}