using UnityEngine;

namespace CS464.A3
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxHearts = 3;
        [SerializeField, Min(0f)] private float iFrames = 1f;
        private int current;
        private float invulnerableUntil;
        private Renderer[] visuals;
        public int Current => current;
        public int Maximum => maxHearts;

        private void Awake()
        {
            current = maxHearts;
            visuals = GetComponentsInChildren<Renderer>();
        }

        private void Update()
        {
            bool visible = Time.time >= invulnerableUntil || Mathf.FloorToInt(Time.time * 12f) % 2 == 0;
            foreach (Renderer visual in visuals)
                if (visual != null) visual.enabled = visible;
        }

        public bool TakeDamage(int damage)
        {
            GameManager manager = GameManager.Instance;
            if (damage <= 0 || current <= 0 || Time.time < invulnerableUntil ||
                manager == null || manager.HasEnded || manager.IsRespawning) return false;
            current = Mathf.Max(0, current - damage);
            invulnerableUntil = Time.time + iFrames;
            manager.UpdateHealth();
            if (current <= 0) manager.LoseLife();
            return true;
        }

        public void Restore(float protectionSeconds)
        {
            current = maxHearts;
            invulnerableUntil = Time.time + protectionSeconds;
            GameManager.Instance.UpdateHealth();
        }
    }
}
