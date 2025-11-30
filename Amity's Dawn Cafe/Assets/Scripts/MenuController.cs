using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI")]
    public GameObject menuCanvas;

    [Header("Video")]
    public VideoPlayer videoPlayer;      // Assign your VideoPlayer here
    public bool pauseMusicDuringVideo = true;

    void Start()
    {
        menuCanvas.SetActive(true);

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
                return;

            ToggleMenu();
        }
    }

    // Public method for UI button
    public void ToggleMenu()
    {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
        PauseController.SetPause(menuCanvas.activeSelf);
    }

    // --- Quit Game ---
    public static void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // --- VIDEO TRIGGERED HERE ---
    public void PlayTriggeredVideo()
    {
        if (videoPlayer == null) return;

        // pause background music
        if (pauseMusicDuringVideo)
            SoundEffectManager.PauseMusic();

        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // resume background music
        if (pauseMusicDuringVideo)
            SoundEffectManager.ResumeMusic();
    }
}
