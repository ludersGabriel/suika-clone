using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameoverMenu : MonoBehaviour {
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Image overlay;
    [SerializeField] private float fadeSeconds = 0.35f;
    [SerializeField] private float growScale = 1.08f;

    void Start() {
        canvas.gameObject.SetActive(false);
        if (overlay) {
            var c = overlay.color;
            c.a = 0f;
            overlay.color = c;
            overlay.rectTransform.localScale = Vector3.one;
            overlay.raycastTarget = true;
            overlay.transform.SetAsLastSibling();
            overlay.gameObject.SetActive(false);
        }
    }

    void OnEnable() => GameManager.OnGameOver += HandleGameOver;
    void OnDisable() => GameManager.OnGameOver -= HandleGameOver;

    private void HandleGameOver() {
        canvas.gameObject.SetActive(true);
        finalScoreText.text = $"Final Score\n{GameManager.Score}";
    }

    public void ExitBtn() => Application.Quit();
    public void MainMenuBtn() => StartCoroutine(FadeThenLoad(0));
    public void RetryBtn() => StartCoroutine(FadeThenLoad(SceneManager.GetActiveScene().buildIndex));

    IEnumerator FadeThenLoad(int buildIndex) {
        if (overlay) {
            overlay.gameObject.SetActive(true);
            float t = 0f;
            while (t < fadeSeconds) {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / fadeSeconds);
                var c = overlay.color;
                c.a = k;
                overlay.color = c;
                overlay.rectTransform.localScale = Vector3.one * Mathf.Lerp(1f, growScale, k);
                yield return null;
            }
        } else {
            yield return null;
        }

        var op = SceneManager.LoadSceneAsync(buildIndex);
        op.allowSceneActivation = true;
    }
}
