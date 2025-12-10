using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject menuCanvas;
    public GameObject GOCanvas;
    public TMP_Text finalScoreText;

    private void Start()
    {
        menuCanvas.SetActive(true);

        // Subscribe safely to timer event
        if (GameTimer.Instance != null)
            GameTimer.Instance.OnTimerEnd += TimeUp;
    }

    private void OnDestroy()
    {
        // Unsubscribe safely
        if (GameTimer.Instance != null)
            GameTimer.Instance.OnTimerEnd -= TimeUp;
    }

    private void Update()
    {
        // Toggle menu with Tab key
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
                return;

            ToggleMenu();
        }
    }

    // Restart/Replay the game
    public void OnReplayButton()
    {
        // Unpause the game first
        PauseController.SetPause(false);
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Called when timer reaches zero
    private void TimeUp()
    {
        PauseController.SetPause(true);
        
        // Count active orders that weren't completed
        int failedOrders = OrderController.Instance.FailAllOrdersAndCount();
        int fulfilledOrders = OrderController.Instance.GetFulfilledOrdersCount();
        
        // Apply penalty for accepted but incomplete orders
        if (failedOrders > 0)
        {
            int penaltyPerOrder = 30;
            int totalPenalty = failedOrders * penaltyPerOrder;
            ScoreManager.Instance.SubtractScore(totalPenalty);
        }
        
        // Update the final score display with stats
        if (finalScoreText != null)
        {
            int finalScore = ScoreManager.Instance.GetScore();
            finalScoreText.text = $"Final  Score: {finalScore}\n" +
                                $"Orders  Fulfilled: {fulfilledOrders}\n" +
                                $"Orders  Failed: {failedOrders}";
        }
        
        // Show Game Over canvas
        GOCanvas.SetActive(true);
    }

    // Toggle the menu on/off
    public void ToggleMenu()
    {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
        PauseController.SetPause(menuCanvas.activeSelf);
    }

    // Quit the game
    public static void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    // Play button starts the timer and unpauses
    public void OnPlayButton()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.ResetTimer();  // Optional: reset timer
            GameTimer.Instance.StartTimer();
        }

        PauseController.SetPause(false);
        menuCanvas.SetActive(false);
    }
}
