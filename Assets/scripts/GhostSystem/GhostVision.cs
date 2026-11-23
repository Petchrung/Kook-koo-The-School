using UnityEngine;

// Sight checks field-of-view, range, and solid occluders.
public class GhostVision : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float viewDistance = 12f;
    [SerializeField, Range(1f, 360f)] private float viewAngle = 110f;
    [SerializeField] private float eyeHeight = 1.6f;
    [SerializeField] private float targetHeight = 1.0f;
    [SerializeField] private LayerMask sightLayers = ~0;

    public bool CanSee(Transform target)
    {
        if (target == null) return false;
        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPoint = target.position + Vector3.up * targetHeight;
        Vector3 delta = targetPoint - eye;
        float distance = delta.magnitude;
        if (distance < 0.001f || distance > viewDistance) return false;
        if (Vector3.Angle(transform.forward, delta) > viewAngle * 0.5f)
            return false;

        // RaycastAll lets us ignore the ghost's own colliders, even if in front of its eyes.
        RaycastHit[] hits = Physics.RaycastAll(
            eye, delta.normalized, distance + 0.05f, sightLayers,
            QueryTriggerInteraction.Ignore);

        float nearestDistance = float.PositiveInfinity;
        Transform nearest = null;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
                continue;
            if (hit.distance < nearestDistance)
            {
                nearestDistance = hit.distance;
                nearest = hit.transform;
            }
        }

        // Player needs a non-trigger Collider/CharacterController on self or child.
        return nearest != null && (nearest == target || nearest.IsChildOf(target));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Gizmos.DrawRay(origin, left * viewDistance);
        Gizmos.DrawRay(origin, right * viewDistance);
    }
}
