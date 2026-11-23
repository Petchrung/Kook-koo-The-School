using UnityEngine;
using UnityEngine.Events;

// A catch is reported once. Actual game over or skill-check is handled elsewhere.
public class GhostCatch : MonoBehaviour
{
    [SerializeField, Min(0f)] private float catchDistance = 1.3f;
    public UnityEvent onPlayerCaught;
    private bool caught;

    public bool IsInCatchRange(Vector3 ghostPosition, Vector3 playerPosition)
    {
        return !caught && (ghostPosition - playerPosition).sqrMagnitude <=
               catchDistance * catchDistance;
    }

    public void NotifyCaught()
    {
        if (caught) return;
        caught = true;
        Debug.Log("KOOK-KOO: ghost caught player; connect Skill Check/Game Over here.");
        onPlayerCaught?.Invoke();
    }

    public void ResetCatch() => caught = false;
}
