using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;
    private static AudioSource audioSource;
    private static SoundEffectLibrary soundEffectLibrary;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    // --- NEW: Background music audio source & clip ---
    [SerializeField] private static AudioSource musicSource;
    [SerializeField] private AudioClip musicTrack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // SFX setup
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();

            // MUSIC setup (added)
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = true;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Play(string soundName)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    void Start()
    {
        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });

        // music slider listener (added)
        musicSlider.onValueChanged.AddListener(delegate { OnMusicValueChanged(); });


        // auto-start track
        if (musicSource != null && musicTrack != null)
        {
            musicSource.clip = musicTrack;
            musicSource.loop = true;
            musicSource.Play();
        }

    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    // MUSIC VOLUME (added)
    public static void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // ONE BUTTON FOR BOTH (modified)
    public void ButtonClick()
    {
        bool mute = (audioSource.volume != 0 || musicSource.volume != 0);

        if (mute)
        {
            audioSource.volume = 0;
            musicSource.volume = 0;

            sfxSlider.value = 0;
            musicSlider.value = 0;
        }
        else
        {
            audioSource.volume = 1;
            musicSource.volume = 1;

            sfxSlider.value = 1;
            musicSlider.value = 1;
        }
    }

    public void ClickSound()
    {
        Play("Machine");
    }

    public static void PlayMusic(AudioClip newTrack)
    {
        if (musicSource == null || newTrack == null)
            return;

        musicSource.clip = newTrack;
        musicSource.loop = true;
        musicSource.Play();
    }

    public static void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    public static void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.Play();
    }


    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }

    // added
    public void OnMusicValueChanged()
    {
        SetMusicVolume(musicSlider.value);
    }
}
