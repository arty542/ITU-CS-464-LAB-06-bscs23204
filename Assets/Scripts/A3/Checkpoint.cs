using UnityEngine;

namespace CS464.A3
{
    public sealed class Checkpoint : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
                GameManager.Instance.SetCheckpoint(spawnPoint);
        }
    }
}
