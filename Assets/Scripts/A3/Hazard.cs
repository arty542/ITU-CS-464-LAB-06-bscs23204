using UnityEngine;

namespace CS464.A3
{
    // One shared damage script for spikes, guards, crushers and surprise traps.
    [RequireComponent(typeof(Collider))]
    public class Hazard : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockback = 6f;
        [SerializeField] private float knockTime = 0.25f;

        private void OnTriggerStay(Collider other)
        {
            Rigidbody body = other.attachedRigidbody;
            if (body == null || !body.CompareTag("Player")) return;
            Health health = body.GetComponent<Health>();
            if (health == null || !health.TakeDamage(damage) || body.isKinematic) return;
            Vector3 away = body.position - transform.position;
            away.y = 0f;
            if (away.sqrMagnitude < 0.001f) away = -transform.forward;
            body.GetComponent<PlayerController>().Knockback(away.normalized * knockback, knockTime);
        }
    }
}
