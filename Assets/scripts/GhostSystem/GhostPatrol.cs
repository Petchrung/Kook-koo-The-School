using UnityEngine;
using UnityEngine.AI;

public class GhostPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField, Min(0f)] private float walkSpeed = 1.8f;
    [SerializeField, Min(0f)] private float pauseAtPoint = 1f;
    [SerializeField, Min(0.01f)] private float arrivalTolerance = 0.3f;

    private int currentIndex = -1;
    private float waitTimer;

    public void Begin(NavMeshAgent agent)
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = false;
        agent.speed = walkSpeed;
        waitTimer = 0f;
        SetNextDestination(agent);
    }

    public void Tick(NavMeshAgent agent, float dt)
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || agent.pathPending)
            return;

        // If route is not reachable, skip it after a pause rather than freezing forever.
        bool arrived = agent.remainingDistance <= agent.stoppingDistance + arrivalTolerance;
        bool failed = agent.pathStatus == NavMeshPathStatus.PathInvalid ||
                      (!agent.hasPath && !agent.pathPending);
        if (!arrived && !failed) { waitTimer = 0f; return; }
        waitTimer += dt;
        if (waitTimer < pauseAtPoint) return;
        waitTimer = 0f;
        SetNextDestination(agent);
    }

    private void SetNextDestination(NavMeshAgent agent)
    {
        if (!agent.isOnNavMesh || patrolPoints == null || patrolPoints.Length == 0)
            return;
        for (int attempts = 0; attempts < patrolPoints.Length; attempts++)
        {
            currentIndex = (currentIndex + 1) % patrolPoints.Length;
            Transform point = patrolPoints[currentIndex];
            if (point == null) continue;
            agent.SetDestination(point.position);
            return;
        }
    }
}
