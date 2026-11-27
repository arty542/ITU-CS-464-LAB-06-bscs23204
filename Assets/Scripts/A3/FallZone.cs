using UnityEngine;

namespace CS464.A3
{
    public sealed class FallZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
                GameManager.Instance.LoseLife();
        }
    }
}
