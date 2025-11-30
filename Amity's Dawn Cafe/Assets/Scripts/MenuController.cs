using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject menuCanvas;

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

    // Called when timer reaches zero
    private void TimeUp()
    {
        PauseController.SetPause(true);

        // Count failed orders
        int failedOrders = OrderController.Instance.FailAllOrdersAndCount();

        // Apply penalty
        int penaltyPerOrder = 30;
        int totalPenalty = failedOrders * penaltyPerOrder;
        ScoreManager.Instance.SubtractScore(totalPenalty);

        // Show menu as "Game Over"
        menuCanvas.SetActive(true);
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
