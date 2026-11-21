
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GhostAI : MonoBehaviour
{
    public enum GhostState
    {
        Patrol,
        Chase,
        Search,
        Capture
    }

    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float captureDistance = 1.5f;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float loseRange = 15f;
    public float fieldOfView = 120f;
    public float eyeHeight = 1.6f;
    public LayerMask obstacleMask;

    [Header("Search")]
    public float searchDuration = 4f;

    [Header("Debug")]
    public GhostState currentState = GhostState.Patrol;

    private NavMeshAgent agent;
    private int patrolIndex;
    private Vector3 lastKnownPosition;
    private float searchTimer;
    private bool hasDestination;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(
                "Ghost is not on NavMesh. Bake navigation first.");
            enabled = false;
            return;
        }

        agent.speed = patrolSpeed;
        GoToPatrolPoint();
    }

    void Update()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(
            transform.position, player.position);

        bool canSeePlayer = CanSeePlayer(distance);

        if (canSeePlayer)
        {
            lastKnownPosition = player.position;
        }

        switch (currentState)
        {
            case GhostState.Patrol:
                if (canSeePlayer)
                    StartChase();
                else
                    Patrol();
                break;

            case GhostState.Chase:
                if (distance <= captureDistance)
                    CapturePlayer();
                else if (canSeePlayer)
                    agent.SetDestination(player.position);
                else if (distance > loseRange)
                    StartSearch();
                else
                    agent.SetDestination(lastKnownPosition);
                break;

            case GhostState.Search:
                if (canSeePlayer)
                {
                    StartChase();
                    break;
                }

                searchTimer -= Time.deltaTime;

                if (searchTimer <= 0f)
                {
                    currentState = GhostState.Patrol;
                    agent.speed = patrolSpeed;
                    GoToPatrolPoint();
                }
                break;

            case GhostState.Capture:
                // Will connect to PlayerHealth and SkillCheck later.
                break;
        }
    }

    bool CanSeePlayer(float distance)
    {
        if (distance > detectionRange)
            return false;

        Vector3 eyePosition =
            transform.position + Vector3.up * eyeHeight;

        Vector3 targetPosition =
            player.position + Vector3.up * 1f;

        Vector3 direction =
            (targetPosition - eyePosition).normalized;

        float angle = Vector3.Angle(
            transform.forward, direction);

        if (angle > fieldOfView * 0.5f)
            return false;

        float rayDistance = Vector3.Distance(
            eyePosition, targetPosition);

        // Walls must be included in obstacleMask.
        if (Physics.Raycast(
            eyePosition,
            direction,
            rayDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return true;
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (agent.pathPending)
            return;

        if (!hasDestination ||
            agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            GoToPatrolPoint();
        }
    }

    void GoToPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (patrolPoints[patrolIndex] == null)
            return;

        hasDestination = agent.SetDestination(
            patrolPoints[patrolIndex].position);
    }

    void StartChase()
    {
        currentState = GhostState.Chase;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
    }

    void StartSearch()
    {
        currentState = GhostState.Search;
        agent.speed = patrolSpeed;
        searchTimer = searchDuration;
        agent.SetDestination(lastKnownPosition);
    }

    void CapturePlayer()
    {
        currentState = GhostState.Capture;
        agent.isStopped = true;

        Debug.Log("Ghost captured the player!");

        // Later: Start SkillCheck or reduce player health.
    }

    // For future SchoolRuleManager integration.
    public void AlertGhost(Vector3 playerPosition)
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        lastKnownPosition = playerPosition;
        currentState = GhostState.Chase;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(playerPosition);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position, captureDistance);
    }
}
