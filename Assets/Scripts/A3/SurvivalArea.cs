using UnityEngine;

namespace CS464.A3
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class SurvivalArea : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other) => SetInside(other, true);
        private void OnTriggerStay(Collider other) => SetInside(other, true);
        private void OnTriggerExit(Collider other) => SetInside(other, false);

        private void SetInside(Collider other, bool inside)
        {
            if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
                GameManager.Instance.SetSurvivalArea(inside);
        }
    }
}
