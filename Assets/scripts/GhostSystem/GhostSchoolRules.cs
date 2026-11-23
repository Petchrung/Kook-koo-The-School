using UnityEngine;
using UnityEngine.Events;

// Event-driven: other scripts notify this component about rule violations.
// It does NOT automatically read player input, movement or flashlight state.
[RequireComponent(typeof(GhostHearing))]
public class GhostSchoolRules : MonoBehaviour
{
    public enum RuleType { Running, LoudNoise, Flashlight, RestrictedArea }

    [System.Serializable]
    public class RuleViolationEvent : UnityEvent<RuleType> { }

    [Header("Rule Settings")]
    [SerializeField, Min(0f)] private float runningLoudness = 1f;
    [SerializeField, Min(0f)] private float noiseLoudness = 1.5f;
    [SerializeField, Min(0f)] private float flashlightLoudness = 0.8f;
    [SerializeField, Min(0f)] private float restrictedAreaLoudness = 1.2f;
    [SerializeField, Min(0f)] private float eventCooldown = 0.5f;

    public RuleViolationEvent onRuleViolated;

    private GhostHearing hearing;
    private float nextAllowedTime;

    private void Awake() => hearing = GetComponent<GhostHearing>();

    public void ReportViolation(RuleType type, Vector3 playerPosition)
    {
        if (Time.time < nextAllowedTime) return;
        nextAllowedTime = Time.time + eventCooldown;

        float loudness = 1f;
        switch (type)
        {
            case RuleType.Running: loudness = runningLoudness; break;
            case RuleType.LoudNoise: loudness = noiseLoudness; break;
            case RuleType.Flashlight: loudness = flashlightLoudness; break;
            case RuleType.RestrictedArea: loudness = restrictedAreaLoudness; break;
        }

        hearing.ReportNoise(playerPosition, loudness);
        onRuleViolated?.Invoke(type);
    }
}
