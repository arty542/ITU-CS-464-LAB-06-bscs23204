using UnityEngine;

namespace CS464.A3
{
    public sealed class FinishGate : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
                GameManager.Instance.TryFinish();
        }
    }
}
