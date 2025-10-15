using System.ComponentModel;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;

    public static event System.Action<int> OnScoreChanged;
    public static event System.Action OnGameOver;
    public static event System.Action OnGamePause;

    private static bool isGameOver;
    public static bool IsGameOver {
        get => isGameOver;
        private set {
            isGameOver = value;
            if (isGameOver) OnGameOver?.Invoke();
        }
    }

    public static void TriggerGameOver() => IsGameOver = true;


    private static int score;
    public static int Score {
        get => score;
        private set {
            score = value;
            OnScoreChanged?.Invoke(score);
        }
    }

    private static bool isPaused;
    public static bool IsPaused {
        get => isPaused;
        set {
            isPaused = value;
            OnGamePause?.Invoke();
        }
    }

    void OnEnable() {
        OnScoreChanged += HandleScoreChanged;
        OnGameOver += HandleGameOver;
        OnGamePause += HandleGamePause;
    }

    void OnDisable() {
        OnScoreChanged -= HandleScoreChanged;
        OnGameOver -= HandleGameOver;
        OnGamePause -= HandleGamePause;
    }

    void Start() {
        HandleScoreChanged(Score);
        IsGameOver = false;
        IsPaused = false;
    }

    void Update() {
        if (IsGameOver) return;
        if (PlayerInputControl.isCancelPressed) {
            IsPaused = !IsPaused;
        }
    }

    private void HandleScoreChanged(int newScore) {
        if (scoreText) scoreText.text = newScore.ToString();
    }

    public static void AddScore(int amount) {
        Score += amount;
    }

    private void HandleGameOver() {
        Time.timeScale = 0f;
    }

    private void HandleGamePause() {
        Time.timeScale = IsPaused ? 0f : 1f;
    }
}
