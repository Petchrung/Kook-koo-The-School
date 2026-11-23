using UnityEngine;
using UnityEngine.AI;

public class GhostSearch : MonoBehaviour
{
    [SerializeField, Min(0f)] private float searchSpeed = 2f;
    [SerializeField, Min(0.1f)] private float searchDuration = 5f;
    private float timer;

    public void Begin(NavMeshAgent agent, Vector3 lastKnownPosition)
    {
        if (!agent.isOnNavMesh) return;
        agent.isStopped = false;
        agent.speed = searchSpeed;
        timer = searchDuration;
        agent.SetDestination(lastKnownPosition);
    }

    // Returns true when time is up. Vision can interrupt it at any moment.
    public bool Tick(NavMeshAgent agent, float dt)
    {
        timer -= dt;
        return timer <= 0f;
    }
}
