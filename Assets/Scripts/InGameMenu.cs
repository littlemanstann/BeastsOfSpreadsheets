using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

public class InGameMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pauseMenuUI.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                Time.timeScale = 0f; // Pause game time
                pauseMenuUI.SetActive(true); // Show in-game menu
            }
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // Resume game time
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        Time.timeScale = 1f;
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
    }

    public void LevelSelect()
    {
        Time.timeScale = 1f; // Resume game time
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level Select");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume game time
        pauseMenuUI.SetActive(false); // Hide in-game menu
    }
}
