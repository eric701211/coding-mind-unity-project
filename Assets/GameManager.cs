using UnityEngine;
using UnityEngine.SceneManagement; // Required for reloading the scene

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI; // Drag your GameOverScreen panel here

    void Start()
    {
        // Ensure the screen is hidden when the game starts
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    public void TriggerGameOver()
    {
        // 1. Show the Game Over Screen
        gameOverUI.SetActive(true);

        // 2. Freeze the game so zombies stop moving and attacking
        Time.timeScale = 0f; 

        // 3. Unlock and show the mouse cursor so you can click the button
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        // MUST reset time back to normal, otherwise the reloaded scene will be frozen!
        Time.timeScale = 1f; 
        
        // Reloads whatever scene you are currently in
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}