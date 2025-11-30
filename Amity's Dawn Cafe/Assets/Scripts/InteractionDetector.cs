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
        Debug.Log($"OnTriggerEnter2D: hit {collision.gameObject.name}");
        
        // Get ALL MonoBehaviours and check if any implement IInteractable
        MonoBehaviour[] components = collision.GetComponents<MonoBehaviour>();
        Debug.Log($"Found {components.Length} MonoBehaviours");
        
        IInteractable interactable = null;
        
        foreach (MonoBehaviour component in components)
        {
            if (component == null)
            {
                Debug.Log("  -> Null component");
                continue;
            }
            
            Debug.Log($"  -> Checking {component.GetType().Name}, is IInteractable? {component is IInteractable}");
            
            if (component is IInteractable)
            {
                interactable = component as IInteractable;
                Debug.Log($"  -> Found IInteractable on {component.GetType().Name}");
                break;
            }
        }
        
        if (interactable != null)
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(interactable.CanInteract());
        }
        else
        {
            Debug.Log($"  -> No IInteractable found on {collision.gameObject.name}");
        }
    }


        private void OnTriggerExit2D(Collider2D collision)
    {
        MonoBehaviour[] components = collision.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour component in components)
        {
            if (component is IInteractable interactable && interactable == interactableInRange)
            {
                interactableInRange = null;
                interactionIcon.SetActive(false);
                break;
            }
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
