using UnityEngine;
using UnityEngine.AI;

public class GhostChase : MonoBehaviour
{
    [SerializeField, Min(0f)] private float chaseSpeed = 4.2f;
    [SerializeField, Min(0.02f)] private float repathInterval = 0.15f;
    private float repathTimer;

    public void Begin(NavMeshAgent agent)
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        repathTimer = 0f;
    }

    public void Tick(NavMeshAgent agent, Vector3 destination, float dt)
    {
        if (!agent.isOnNavMesh) return;
        repathTimer -= dt;
        if (repathTimer > 0f) return;
        repathTimer = repathInterval;
        agent.SetDestination(destination);
    }
}
