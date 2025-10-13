using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;

    public static event System.Action<int> OnScoreChanged;

    private static int score;
    public static int Score {
        get => score;
        private set {
            score = value;
            OnScoreChanged?.Invoke(score);
        }
    }

    void OnEnable() {
        OnScoreChanged += HandleScoreChanged;
    }

    void OnDisable() {
        OnScoreChanged -= HandleScoreChanged;
    }

    void Start() {
        HandleScoreChanged(Score);
    }

    private void HandleScoreChanged(int newScore) {
        if (scoreText) scoreText.text = newScore.ToString();
    }

    public static void AddScore(int amount) {
        Score += amount;
    }
}
