using UnityEngine;

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
        Time.timeScale = 1f; // Resume game time
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume game time
        pauseMenuUI.SetActive(false); // Hide in-game menu
    }
}
