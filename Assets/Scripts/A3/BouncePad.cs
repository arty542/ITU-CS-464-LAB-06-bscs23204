using UnityEngine;

namespace CS464.A3
{
    [RequireComponent(typeof(Collider))]
    public sealed class BouncePad : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float launchSpeed = 11.5f;
        private void OnTriggerEnter(Collider other)
        {
            Rigidbody body = other.attachedRigidbody;
            if (body == null || !body.CompareTag("Player") || body.isKinematic) return;
            Vector3 velocity = body.linearVelocity;
            body.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
            // One upward kick, independent of player mass. Horizontal steering remains available.
            body.AddForce(Vector3.up * launchSpeed, ForceMode.VelocityChange);
            GameManager.Instance.ShowMessage("Keep moving forward to land on the balcony.", 2.5f);
        }
    }
}
