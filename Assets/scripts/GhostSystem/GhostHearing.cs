using UnityEngine;

// Other gameplay scripts may call ReportNoise when footsteps/doors/etc make noise.
[RequireComponent(typeof(KookKooGhostAI))]
public class GhostHearing : MonoBehaviour
{
    [SerializeField, Min(0f)] private float hearingRadius = 12f;
    [SerializeField, Min(0f)] private float minimumLoudness = 0.1f;
    private KookKooGhostAI brain;

    private void Awake() => brain = GetComponent<KookKooGhostAI>();

    // loudness 1 = normal, 2 = twice as far. Noise is not generated automatically.
    public bool ReportNoise(Vector3 sourcePosition, float loudness = 1f)
    {
        if (loudness < minimumLoudness) return false;
        float effectiveRadius = hearingRadius * Mathf.Max(0f, loudness);
        if ((sourcePosition - transform.position).sqrMagnitude >
            effectiveRadius * effectiveRadius) return false;
        brain.Investigate(sourcePosition);
        return true;
    }
}
