using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CS464.A3
{
    public enum LevelObjective { CoinTarget, TimedEscape, CollectAll, Survive, TimedCollect }

    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField, Min(1)] private int targetScore = 8;
        [SerializeField, Min(1)] private int startingLives = 3;
        [SerializeField] private LevelObjective objectiveType = LevelObjective.CoinTarget;
        [SerializeField, Min(1f)] private float timeLimit = 60f;
        [SerializeField, Min(1f)] private float survivalDuration = 30f;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private PlayerController player;
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text livesText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private Button winRestart;
        [SerializeField] private Button loseRestart;
        [SerializeField, Min(0f)] private float respawnDelay = 0.8f;
        [SerializeField] private string levelLabel = "01 / TRAINING COURTYARD";
        [SerializeField] private string lesson = "COLLECT / BOUNCE / FINISH";
        [SerializeField] private string objective = "Collect 8 coins, ride the bounce pad, then reach FINISH.";
        [SerializeField] private TMP_Text levelHeading;
        [SerializeField] private TMP_Text lessonText;
        [SerializeField] private TMP_Text winEyebrow;
        [SerializeField] private TMP_Text loseEyebrow;
        [SerializeField] private TMP_Text winTitle;
        [SerializeField] private TMP_Text winDetail;
        [SerializeField] private TMP_Text loseTitle;
        [SerializeField] private TMP_Text loseDetail;
        [SerializeField] private TMP_Text boostText;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private TMP_Text nextLevelText;

        private Rigidbody body;
        private Health health;
        private int score;
        private int lives;
        private bool respawning;
        private bool ended;
        private float messageUntil;
        private int lastBoostSeconds = -1;
        private float remainingTime;
        private float survivalElapsed;
        private bool insideSurvivalArea;
        private string lastTimerLabel;

        public int Score => score;
        public int TargetScore => targetScore;
        public int Lives => lives;
        public bool HasEnded => ended;
        public bool IsRespawning => respawning;
        public Transform RespawnPoint => respawnPoint;
        public LevelObjective ObjectiveType => objectiveType;
        public float RemainingTime => remainingTime;
        public float SurvivalElapsed => survivalElapsed;
        public bool InsideSurvivalArea => insideSurvivalArea;
        private bool UsesCoins => objectiveType == LevelObjective.CoinTarget ||
            objectiveType == LevelObjective.CollectAll || objectiveType == LevelObjective.TimedCollect;
        private bool IsTimed => objectiveType == LevelObjective.TimedEscape ||
            objectiveType == LevelObjective.TimedCollect;
        private bool ObjectiveComplete => (!UsesCoins || score >= targetScore) &&
            (objectiveType != LevelObjective.Survive || survivalElapsed >= survivalDuration);

        private void Awake()
        {
            Instance = this;
            Time.timeScale = 1f;
            lives = startingLives;
            remainingTime = timeLimit;
            body = player.GetComponent<Rigidbody>();
            health = player.GetComponent<Health>();
            winPanel.SetActive(false);
            losePanel.SetActive(false);
            winRestart.onClick.AddListener(Restart);
            loseRestart.onClick.AddListener(Restart);
            if (nextLevelButton != null) nextLevelButton.onClick.AddListener(NextLevel);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Start()
        {
            if (objectiveType == LevelObjective.CollectAll)
                targetScore = FindObjectsByType<Coin>(FindObjectsSortMode.None).Length;
            // All Awake methods, including Health's, have completed now.
            UpdateScore();
            UpdateHealth();
            livesText.text = $"LIVES  {lives}";
            if (levelHeading != null) levelHeading.text = levelLabel;
            if (lessonText != null) lessonText.text = lesson;
            if (winEyebrow != null) winEyebrow.text = levelLabel;
            if (loseEyebrow != null) loseEyebrow.text = levelLabel;
            if (winTitle != null) winTitle.text = SceneManager.GetActiveScene().buildIndex == 4
                ? "ALL FIVE LEVELS CLEARED" : "LEVEL CLEARED";
            if (winDetail != null) winDetail.text = objectiveType == LevelObjective.Survive
                ? "You survived the arena and reached the exit."
                : IsTimed ? "You completed the objective before time ran out."
                : "You collected the required coins and reached the exit.";
            if (nextLevelText != null) nextLevelText.text = SceneManager.GetActiveScene().buildIndex == 4
                ? "BACK TO LEVEL01" : "NEXT LEVEL";
            SetDefaultMessage();
            UpdateTimer();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                Restart();
                return;
            }
            if (!ended && IsTimed)
            {
                remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);
                if (remainingTime <= 0f)
                    LoseGame("TIME'S UP", "Reach the exit before the countdown ends.");
            }
            if (!ended && !respawning && insideSurvivalArea &&
                objectiveType == LevelObjective.Survive && survivalElapsed < survivalDuration)
            {
                survivalElapsed = Mathf.Min(survivalDuration, survivalElapsed + Time.deltaTime);
                if (survivalElapsed >= survivalDuration)
                    ShowMessage("Survival complete. Reach the green FINISH gate.", 4f);
            }
            UpdateTimer();
            if (!ended && messageUntil > 0f && Time.unscaledTime >= messageUntil)
                SetDefaultMessage();
            int seconds = Mathf.CeilToInt(player.BoostSecondsRemaining);
            if (boostText != null && seconds != lastBoostSeconds)
            {
                boostText.text = seconds > 0 ? $"SPEED x{player.SpeedMultiplier:0.0} / {seconds}s" : "SPEED / READY";
                lastBoostSeconds = seconds;
            }
        }

        public void AddScore(int amount)
        {
            if (ended || respawning || amount <= 0) return;
            score += amount;
            UpdateScore();
            if (UsesCoins && score == targetScore)
                ShowMessage("Gate unlocked. Follow the green FINISH marker.", 4f);
        }

        public bool TryFinish()
        {
            if (ended || respawning) return false;
            if (!ObjectiveComplete)
            {
                ShowMessage(objectiveType == LevelObjective.Survive
                    ? $"Survive {Mathf.CeilToInt(survivalDuration - survivalElapsed)} more seconds inside the arena."
                    : $"Find {targetScore - score} more coin(s), then return to the gate.", 3f);
                return false;
            }
            ended = true;
            StopPlayer();
            winPanel.SetActive(true);
            Time.timeScale = 0f;
            winRestart.Select();
            return true;
        }

        public void LoseLife()
        {
            if (ended || respawning) return;
            respawning = true;
            insideSurvivalArea = false;
            if (objectiveType == LevelObjective.Survive) survivalElapsed = 0f;
            lives = Mathf.Max(0, lives - 1);
            livesText.text = $"LIVES  {lives}";
            StopPlayer();
            if (lives <= 0)
            {
                LoseGame("OUT OF LIVES", "Restart the level and try the route again.");
            }
            else
            {
                ShowMessage("Take a breath. Returning to your last checkpoint...", respawnDelay + 1f);
                StartCoroutine(Respawn());
            }
        }

        private void LoseGame(string title, string detail)
        {
            if (ended) return;
            ended = true;
            respawning = false;
            StopPlayer();
            if (loseTitle != null) loseTitle.text = title;
            if (loseDetail != null) loseDetail.text = detail;
            losePanel.SetActive(true);
            Time.timeScale = 0f;
            loseRestart.Select();
        }

        public void SetSurvivalArea(bool inside)
        {
            if (!ended && !respawning) insideSurvivalArea = inside;
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(respawnDelay);
            if (ended) yield break;
            body.position = respawnPoint.position;
            body.rotation = respawnPoint.rotation;
            body.isKinematic = false;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            health.Restore(1.5f);
            player.enabled = true;
            FindFirstObjectByType<CameraFollow>()?.SnapToTarget();
            respawning = false;
        }

        private void StopPlayer()
        {
            player.ClearBoost();
            player.enabled = false;
            if (!body.isKinematic) body.linearVelocity = Vector3.zero;
            body.isKinematic = true;
        }

        public void SetCheckpoint(Transform point)
        {
            if (ended || respawning || respawnPoint == point) return;
            respawnPoint = point;
            ShowMessage("Checkpoint reached. Your coins are safe if you fall.", 3f);
        }

        public void UpdateHealth()
        {
            if (health == null) health = player.GetComponent<Health>();
            healthText.text = $"HEARTS  {Mathf.Max(0, health.Current)} / {health.Maximum}";
        }

        public void ShowMessage(string message, float seconds = 3f)
        {
            messageText.text = message;
            messageUntil = Time.unscaledTime + seconds;
        }

        private void SetDefaultMessage()
        {
            messageUntil = 0f;
            messageText.text = ObjectiveComplete && objectiveType != LevelObjective.TimedEscape
                ? "Gate unlocked - head to FINISH."
                : objective;
        }

        private void UpdateScore() => scoreText.text = UsesCoins
            ? $"{score:00} <color=#99A9AF>/ {targetScore:00}</color>"
            : $"{score:00} <color=#99A9AF> BONUS</color>";

        private void UpdateTimer()
        {
            if (timerText == null) return;
            string label;
            if (IsTimed)
            {
                int seconds = Mathf.CeilToInt(remainingTime);
                label = $"TIME  {seconds / 60:00}:{seconds % 60:00}";
            }
            else if (objectiveType == LevelObjective.Survive)
                label = survivalElapsed >= survivalDuration ? "ARENA CLEARED"
                    : $"SURVIVE  {Mathf.CeilToInt(survivalDuration - survivalElapsed)}s" +
                        (insideSurvivalArea ? "" : " / ENTER ARENA");
            else label = objectiveType == LevelObjective.CollectAll ? "COLLECT EVERY COIN" : $"COLLECT {targetScore} + EXIT";
            if (label == lastTimerLabel) return;
            timerText.text = label;
            lastTimerLabel = label;
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().path);
        }

        public void NextLevel()
        {
            Time.timeScale = 1f;
            int index = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            SceneManager.LoadScene(index);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
