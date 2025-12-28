using UnityEngine;
using UnityEngine.InputSystem;

public class CoffeeMachine : MonoBehaviour, IInteractable
{
    [Header("UI References")]
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
        return !isInteracted;
    }

    public void Interact()
    {
        if (!CanInteract() || baristaCanvas == null)
            return;

        isInteracted = true;
        OpenBaristaMenu();
    }

    private void OpenBaristaMenu()
    {
        baristaCanvas.SetActive(true);
        PauseController.SetPause(true);
    }

    public void CloseBaristaMenu()
    {
        if (baristaCanvas == null)
            return;

        baristaCanvas.SetActive(false);
        PauseController.SetPause(false);
        isInteracted = false; // allow re-interaction after closing
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && baristaCanvas.activeSelf)
        {
            CloseBaristaMenu();
        }
    }

}
