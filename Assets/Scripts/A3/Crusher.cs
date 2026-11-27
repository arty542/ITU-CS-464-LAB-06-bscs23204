using System.Collections;
using UnityEngine;

namespace CS464.A3
{
    [RequireComponent(typeof(Collider))]
    public class Crusher : MonoBehaviour
    {
        [SerializeField] private GameObject warning;
        [SerializeField] private GameObject slab;
        [SerializeField] private float restTime = 2.2f;
        [SerializeField] private float warnTime = 0.8f;
        [SerializeField] private float strikeTime = 0.65f;
        private Collider strike;
        public bool IsStriking => strike != null && strike.enabled;

        private void Awake()
        {
            strike = GetComponent<Collider>();
            strike.enabled = false;
            warning.SetActive(false);
            slab.SetActive(false);
        }

        private IEnumerator Start()
        {
            while (true)
            {
                strike.enabled = false;
                slab.SetActive(false);
                warning.SetActive(false);
                yield return new WaitForSeconds(restTime);
                warning.SetActive(true);
                yield return new WaitForSeconds(Mathf.Max(0.5f, warnTime));
                warning.SetActive(false);
                slab.SetActive(true);
                strike.enabled = true;
                yield return new WaitForSeconds(strikeTime);
            }
        }
    }
}
