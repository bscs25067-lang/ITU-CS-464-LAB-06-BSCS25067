using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockback = 6f;


    void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out Health health) || !health.TakeDamage(damage))
            return;

        // Apply knockback if the object has a Rigidbody
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;

        // Calculate push direction away from the spike's center
        Vector3 away = body.position - transform.position;
        away.y = 0f; // Keep knockback strictly horizontal

        // Prevent zero-vector errors if perfectly centered
        if (away.sqrMagnitude < 0.001f)
            away = Vector3.back;

        body.AddForce(away.normalized * knockback, ForceMode.Impulse);
    }
}