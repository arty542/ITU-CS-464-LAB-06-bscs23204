using UnityEngine;

namespace CS464.A3
{
    [RequireComponent(typeof(Collider))]
    public sealed class Coin : MonoBehaviour
    {
        [SerializeField] private Transform visual;
        private Vector3 restPosition;
        private bool collected;
        public bool Collected => collected;

        private void Awake() => restPosition = visual.localPosition;

        private void Update()
        {
            visual.Rotate(Vector3.up, 75f * Time.deltaTime, Space.World);
            visual.localPosition = restPosition + Vector3.up * (Mathf.Sin(Time.time * 2.5f) * 0.12f);
        }

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody body = other.attachedRigidbody;
            if (collected || body == null || !body.CompareTag("Player")) return;
            GameManager manager = GameManager.Instance;
            if (manager == null || manager.HasEnded || manager.IsRespawning) return;
            collected = true;
            GetComponent<Collider>().enabled = false;
            manager.AddScore(1);
            gameObject.SetActive(false);
        }
    }
}
