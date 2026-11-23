using UnityEngine;
using UnityEngine.AI;

// Central controller. Keep this class separate from any existing GhostAI.cs.
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(GhostVision))]
[RequireComponent(typeof(GhostHearing))]
[RequireComponent(typeof(GhostPatrol))]
[RequireComponent(typeof(GhostChase))]
[RequireComponent(typeof(GhostSearch))]
[RequireComponent(typeof(GhostSchoolRules))]
[RequireComponent(typeof(GhostCatch))]
[RequireComponent(typeof(GhostAnimation))]
public class KookKooGhostAI : MonoBehaviour
{
    public enum GhostState { Patrol, Chase, Search, Catch }

    [Header("References")]
    [SerializeField] private Transform player;
    [Header("Behavior")]
    [SerializeField, Min(0f)] private float loseSightDelay = 1.2f;

    public GhostState CurrentState { get; private set; } = GhostState.Patrol;
    public Transform Player => player;
    public Vector3 LastKnownPosition { get; private set; }

    private NavMeshAgent agent;
    private GhostVision vision;
    private GhostPatrol patrol;
    private GhostChase chase;
    private GhostSearch search;
    private GhostCatch ghostCatch;
    private GhostAnimation ghostAnimation;
    private float lastSeenTime = float.NegativeInfinity;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<GhostVision>();
        patrol = GetComponent<GhostPatrol>();
        chase = GetComponent<GhostChase>();
        search = GetComponent<GhostSearch>();
        ghostCatch = GetComponent<GhostCatch>();
        ghostAnimation = GetComponent<GhostAnimation>();
    }

    private void Start()
    {
        if (player != null) LastKnownPosition = player.position;
        if (agent.isOnNavMesh) patrol.Begin(agent);
    }

    private void Update()
    {
        ghostAnimation.UpdateAnimation(agent, CurrentState);
        if (player == null || !agent.isOnNavMesh || CurrentState == GhostState.Catch)
            return;

        bool canSeePlayer = vision.CanSee(player);
        if (canSeePlayer)
        {
            LastKnownPosition = player.position;
            lastSeenTime = Time.time;

            if (CurrentState != GhostState.Chase)
            {
                CurrentState = GhostState.Chase;
                chase.Begin(agent);
            }

            if (ghostCatch.IsInCatchRange(transform.position, player.position))
            {
                CurrentState = GhostState.Catch;
                agent.isStopped = true;
                agent.ResetPath();
                ghostCatch.NotifyCaught();
                return;
            }
        }

        switch (CurrentState)
        {
            case GhostState.Patrol:
                patrol.Tick(agent, Time.deltaTime);
                break;
            case GhostState.Chase:
                // During the grace period, chase the last *seen* position.
                chase.Tick(agent, LastKnownPosition, Time.deltaTime);
                if (!canSeePlayer && Time.time - lastSeenTime >= loseSightDelay)
                    BeginSearch(LastKnownPosition);
                break;
            case GhostState.Search:
                if (search.Tick(agent, Time.deltaTime))
                    BeginPatrol();
                break;
        }
    }

    // Called by hearing/rule systems; not omniscient, so sound causes investigation.
    public void Investigate(Vector3 worldPosition)
    {
        if (CurrentState == GhostState.Catch || CurrentState == GhostState.Chase)
            return;
        if (!agent.isOnNavMesh)
            return;
        LastKnownPosition = worldPosition;
        BeginSearch(worldPosition);
    }

    public void SetPlayer(Transform target) => player = target;

    // Hook a skill-check result into this: true = player escaped; false = still caught.
    public void ResolveCatch(bool escaped)
    {
        if (CurrentState != GhostState.Catch || !escaped) return;
        ghostCatch.ResetCatch();
        if (agent.isOnNavMesh) BeginSearch(LastKnownPosition);
        else CurrentState = GhostState.Search;
    }

    public void ResetAI()
    {
        ghostCatch.ResetCatch();
        lastSeenTime = float.NegativeInfinity;
        CurrentState = GhostState.Patrol;
        if (agent.isOnNavMesh) patrol.Begin(agent);
    }

    private void BeginPatrol()
    {
        CurrentState = GhostState.Patrol;
        patrol.Begin(agent);
    }

    private void BeginSearch(Vector3 target)
    {
        CurrentState = GhostState.Search;
        search.Begin(agent, target);
    }
}
