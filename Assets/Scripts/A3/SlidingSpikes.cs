using System.Collections;
using UnityEngine;

namespace CS464.A3
{
    [RequireComponent(typeof(Rigidbody))]
    public class SlidingSpikes : MonoBehaviour
    {
        [SerializeField] private GameObject spikes;
        [SerializeField] private GameObject warning;
        [SerializeField] private float detectRange = 5f;
        [SerializeField] private float warningTime = 0.5f;
        [SerializeField] private float activeTime = 1.3f;
        [SerializeField] private float slideSpeed = 3f;
        [SerializeField] private float minX = -4f;
        [SerializeField] private float maxX = 4f;
        private PlayerController player;
        private Rigidbody body;
        private Collider danger;
        private Vector3 home;
        private bool busy;
        public bool IsTracking { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            danger = GetComponent<Collider>();
            home = transform.position;
            danger.enabled = false;
            spikes.SetActive(false);
            warning.SetActive(false);
        }

        private void Start() => player = FindAnyObjectByType<PlayerController>();

        private void Update()
        {
            if (busy || GameManager.Instance.HasEnded || GameManager.Instance.IsRespawning) return;
            Vector3 delta = player.transform.position - home;
            delta.y = 0f;
            if (delta.magnitude < detectRange && !player.IsGrounded &&
                player.GetComponent<Rigidbody>().linearVelocity.y > 0.5f) StartCoroutine(Slide());
        }

        private void FixedUpdate()
        {
            if (!IsTracking) return;
            Vector3 target = new Vector3(Mathf.Clamp(player.transform.position.x, minX, maxX), home.y, home.z);
            body.MovePosition(Vector3.MoveTowards(body.position, target, slideSpeed * Time.fixedDeltaTime));
        }

        private IEnumerator Slide()
        {
            busy = true;
            warning.SetActive(true);
            yield return new WaitForSeconds(warningTime);
            warning.SetActive(false);
            spikes.SetActive(true);
            danger.enabled = true;
            IsTracking = true;
            yield return new WaitForSeconds(activeTime);
            IsTracking = false;
            danger.enabled = false;
            spikes.SetActive(false);
            yield return new WaitForSeconds(2f);
            body.position = home;
            busy = false;
        }
    }
}
