using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null; // closest interactable
    public GameObject interactionIcon;
    /*
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.performed)
        {
            interactableInRange?.Interact();
            if (!interactableInRange.CanInteract())
            {
                interactionIcon.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange) {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
    */
    private void Update()
    {
        // Auto-cleanup: Unity “nulls out” destroyed objects but keeps a ghost reference
        if (interactableInRange != null && interactableInRange.Equals(null))
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    //Debug.Log($"OnTriggerEnter2D: hit {collision.gameObject.name}");

    // Try the safe GetComponent approach (works reliably with interfaces)
    var interactable = collision.GetComponent<IInteractable>();
    if (interactable != null)
    {
        //Debug.Log($"  -> Found IInteractable on {collision.gameObject.name}");
        interactableInRange = interactable;
        interactionIcon.SetActive(interactable.CanInteract());
    }
    else
    {
        //Debug.Log($"  -> No IInteractable on {collision.gameObject.name}. Components: {string.Join(", ", System.Array.ConvertAll(collision.gameObject.GetComponents<Component>(), c => c.GetType().Name))}");
    }
}


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (interactableInRange == null)
            return;

        try
        {
            interactableInRange.Interact();

            // Clear the reference immediately if object destroyed itself
            if (interactableInRange == null || interactableInRange.Equals(null) || !interactableInRange.CanInteract())
            {
                interactableInRange = null;
                interactionIcon.SetActive(false);
            }
        }
        catch (MissingReferenceException)
        {
            // Object destroyed mid-interaction
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Interaction failed: {e}");
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
