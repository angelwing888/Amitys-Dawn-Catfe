using UnityEngine;
using TMPro;
using System;

public class GameTimer : MonoBehaviour
{
    // Singleton instance
    public static GameTimer Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("Timer Settings")]
    public float totalTime = 60f;     // Total countdown time
    private bool timerActive = false; // Is the timer running?
    public float timeRemaining;       // Current remaining time

    [Header("UI Settings")]
    public TextMeshProUGUI timerText; // Assign TMP text in Inspector

    public Action OnTimerEnd;         // Event triggered when timer ends

    private void Start()
    {
        ResetTimer();
        UpdateTimerUI(); // show initial value
    }

    private void Update()
    {
        if (!timerActive) return;

        if (PauseController.IsGamePaused) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerActive = false;
            OnTimerEnd?.Invoke();
        }

        UpdateTimerUI();
    }

    // Start the countdown
    public void StartTimer()
    {
        timerActive = true;
    }

    // Stop and reset timer
    public void ResetTimer()
    {
        timerActive = false;
        timeRemaining = totalTime;
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // Optional read-only property
    public float TimeRemaining => timeRemaining;
}
