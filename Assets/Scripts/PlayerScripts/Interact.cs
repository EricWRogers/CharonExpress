
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public interface IInteractable
{
    void Interacting();
    void OnTouchingPlayer();
    void OnNotTouchingPlayer();
}

public interface IInteractableAvailability
{
    bool CanInteract { get; }
}

public class Interact : MonoBehaviour
{
    private IInteractable currentInteractable;
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;

    public static bool interactionsLocked;

    void Start()
    {
        interactionUIActive(false);
    }

    void Update()
    {
        //if (Keyboard.current.eKey.wasPressedThisFrame && currentInteractable != null)
        //{

        //}
    }

    public void OnInteract()
    {
        if (interactionsLocked || currentInteractable == null)
            return;

        IInteractable interactable = currentInteractable;

        interactionUIActive(false);
        currentInteractable = null;

        interactable.OnNotTouchingPlayer();
        interactable.Interacting();
    }

    private void OnTriggerStay(Collider collision)
    {
        if (interactionsLocked)
            return;

        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            if (interactable is IInteractableAvailability availability && !availability.CanInteract)
            {
                if (currentInteractable == interactable)
                {
                    interactionUIActive(false);
                    currentInteractable.OnNotTouchingPlayer();
                    currentInteractable = null;
                }

                return;
            }

            if (currentInteractable == interactable)
                return;

            if (currentInteractable != null)
                currentInteractable.OnNotTouchingPlayer();

            currentInteractable = interactable;
            interactionUIActive(true);
            currentInteractable.OnTouchingPlayer();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null && interactable == currentInteractable)
        {
            interactionUIActive(false);

            currentInteractable.OnNotTouchingPlayer();
            currentInteractable = null;
        }
    }

    public void interactionUIActive(bool active)
    {
        interactionUI.SetActive(active);
    }

    public static void SetInteractionLock(bool locked)
    {
        interactionsLocked = locked;
    }
}
