using UnityEngine;

public class PauseMenu : MonoBehaviour {

    [SerializeField] private Canvas canvas;

    void Start() {
        canvas.gameObject.SetActive(false);
    }

    void OnEnable() {
        GameManager.OnGamePause += HandleGamePause;
    }

    void OnDisable() {
        GameManager.OnGamePause -= HandleGamePause;
    }

    private void HandleGamePause() {
        canvas.gameObject.SetActive(GameManager.IsPaused);
    }

    public void ResumeBtn() {
        GameManager.IsPaused = false;
    }

    public void ExitBtn() {
        Application.Quit();
    }

    public void MainMenuBtn() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
