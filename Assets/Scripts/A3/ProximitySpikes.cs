using System.Collections;
using UnityEngine;

namespace CS464.A3
{
    // Trap-game variation: the damage remains the same shared Hazard component.
    public class ProximitySpikes : MonoBehaviour
    {
        [SerializeField] private Transform spikes;
        [SerializeField] private GameObject warning;
        [SerializeField] private float revealRange = 4f;
        [SerializeField] private float warningTime = 0.5f;
        [SerializeField] private float riseTime = 0.18f;
        [SerializeField] private float activeTime = 1.6f;
        [SerializeField] private float cooldown = 2f;
        private Collider danger;
        private Transform player;
        private bool busy;
        public bool IsRaised => danger != null && danger.enabled;

        private void Awake()
        {
            danger = GetComponent<Collider>();
            danger.enabled = false;
            spikes.gameObject.SetActive(false);
            warning.SetActive(false);
        }

        private void Start() => player = FindAnyObjectByType<PlayerController>().transform;

        private void Update()
        {
            if (busy || GameManager.Instance.HasEnded || GameManager.Instance.IsRespawning) return;
            Vector3 delta = player.position - transform.position;
            delta.y = 0f;
            if (delta.magnitude < revealRange) StartCoroutine(Pop());
        }

        private IEnumerator Pop()
        {
            busy = true;
            warning.SetActive(true);
            yield return new WaitForSeconds(warningTime);
            warning.SetActive(false);
            spikes.localPosition = Vector3.down * 0.8f;
            spikes.gameObject.SetActive(true);
            for (float t = 0; t < riseTime; t += Time.deltaTime)
            {
                spikes.localPosition = Vector3.Lerp(Vector3.down * 0.8f, Vector3.zero, t / riseTime);
                yield return null;
            }
            spikes.localPosition = Vector3.zero;
            danger.enabled = true;
            yield return new WaitForSeconds(activeTime);
            danger.enabled = false;
            spikes.gameObject.SetActive(false);
            yield return new WaitForSeconds(cooldown);
            busy = false;
        }
    }
}
