using UnityEngine;
using UnityEngine.AI;

// Optional Animator integration. Safe if no Animator is assigned.
public class GhostAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string chasingParameter = "IsChasing";
    [SerializeField] private string stateParameter = "GhostState";

    private bool hasSpeed;
    private bool hasChasing;
    private bool hasState;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null) return;
        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == speedParameter && p.type == AnimatorControllerParameterType.Float)
                hasSpeed = true;
            if (p.name == chasingParameter && p.type == AnimatorControllerParameterType.Bool)
                hasChasing = true;
            if (p.name == stateParameter && p.type == AnimatorControllerParameterType.Int)
                hasState = true;
        }
    }

    public void UpdateAnimation(NavMeshAgent agent, KookKooGhostAI.GhostState state)
    {
        if (animator == null) return;
        if (hasSpeed) animator.SetFloat(speedParameter, agent.velocity.magnitude);
        if (hasChasing) animator.SetBool(chasingParameter,
            state == KookKooGhostAI.GhostState.Chase);
        if (hasState) animator.SetInteger(stateParameter, (int)state);
    }
}
