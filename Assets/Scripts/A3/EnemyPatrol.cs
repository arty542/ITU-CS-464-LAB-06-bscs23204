using TMPro;
using UnityEngine;

namespace CS464.A3
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyPatrol : MonoBehaviour
    {
        public enum GuardState { Patrol, Chase, Return }
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private bool pingPong;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float chaseSpeed = 3.5f;
        [SerializeField] private float chaseRange = 4f;
        [SerializeField] private float giveUpRange = 7f;
        [SerializeField] private float arriveDistance = 0.15f;
        [SerializeField] private TMP_Text stateText;
        private Rigidbody body;
        private Transform player;
        private int waypoint;
        private int direction = 1;
        public GuardState State { get; private set; }
        public int CurrentWaypoint => waypoint;

        private void Awake() => body = GetComponent<Rigidbody>();

        private void Start()
        {
            PlayerController controller = FindAnyObjectByType<PlayerController>();
            if (controller != null) player = controller.transform;
            ChangeState(GuardState.Patrol);
        }

        private void FixedUpdate()
        {
            if (player == null || waypoints == null || waypoints.Length < 3 ||
                GameManager.Instance == null || GameManager.Instance.HasEnded ||
                GameManager.Instance.IsRespawning) return;
            Vector3 toPlayer = player.position - body.position;
            toPlayer.y = 0f;
            float distance = toPlayer.magnitude;
            if (State != GuardState.Chase && distance < chaseRange) ChangeState(GuardState.Chase);
            else if (State == GuardState.Chase && distance > giveUpRange) ChangeState(GuardState.Return);

            Vector3 target = State == GuardState.Chase ? player.position : waypoints[waypoint].position;
            target.y = body.position.y;
            float speed = State == GuardState.Chase ? chaseSpeed : patrolSpeed;
            body.MovePosition(Vector3.MoveTowards(body.position, target, speed * Time.fixedDeltaTime));
            if (State == GuardState.Chase || Vector3.Distance(body.position, target) >= arriveDistance) return;
            if (State == GuardState.Return) { ChangeState(GuardState.Patrol); return; }
            if (pingPong)
            {
                if (waypoint == waypoints.Length - 1) direction = -1;
                else if (waypoint == 0) direction = 1;
                waypoint += direction;
            }
            else waypoint = (waypoint + 1) % waypoints.Length;
        }

        private void ChangeState(GuardState next)
        {
            State = next;
            if (stateText != null) stateText.text = "GUARD / " + next.ToString().ToUpper();
        }
    }
}
