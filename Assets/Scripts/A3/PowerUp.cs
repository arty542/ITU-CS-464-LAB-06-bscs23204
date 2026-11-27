using UnityEngine;

namespace CS464.A3
{
    public class PowerUp : MonoBehaviour
    {
        [SerializeField] private float multiplier = 1.6f;
        [SerializeField] private float duration = 5f;

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody body = other.attachedRigidbody;
            if (body == null || !body.CompareTag("Player") || GameManager.Instance.HasEnded ||
                GameManager.Instance.IsRespawning) return;
            body.GetComponent<PlayerController>().BoostSpeed(multiplier, duration);
            GameManager.Instance.ShowMessage("Speed boost! Another bottle refreshes the 5-second timer.", 3f);
            Destroy(gameObject);
        }
    }
}
