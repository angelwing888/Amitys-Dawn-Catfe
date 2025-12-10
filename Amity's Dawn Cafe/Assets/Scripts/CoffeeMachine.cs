using UnityEngine;
using UnityEngine.InputSystem;

public class CoffeeMachine : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    [SerializeField] private AudioClip baristaMusicTrack;
    [SerializeField] private AudioClip defaultGameMusic; // assign your normal track

    [SerializeField] private GameObject baristaCanvas; // assign your barista UI/menu here

    private bool isInteracted = false;

    private void Start()
    {
        if (baristaCanvas != null)
        {
            baristaCanvas.SetActive(false);
        }
    }

    // === IInteractable Implementation ===
    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (baristaCanvas == null)
            return;
        
        // CHANGED: Toggle between open and closed
        if (baristaCanvas.activeSelf)
        {
            CloseBaristaMenu();
        }
        else
        {
            OpenBaristaMenu();
        }
    }

    private void OpenBaristaMenu()
    {
        baristaCanvas.SetActive(true);
        //PauseController.SetPause(true);
        if (baristaMusicTrack != null)
        {
            SoundEffectManager.PlayMusic(baristaMusicTrack);
        }
    }

    public void CloseBaristaMenu()
    {
        if (baristaCanvas == null)
            return;

        baristaCanvas.SetActive(false);
       //PauseController.SetPause(false);
        isInteracted = false; // allow re-interaction after closing

        if (defaultGameMusic != null)
        {
            SoundEffectManager.PlayMusic(defaultGameMusic);
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && baristaCanvas.activeSelf)
        {
            CloseBaristaMenu();
        }
    }

}
