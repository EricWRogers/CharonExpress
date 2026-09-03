using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public interface IInteractableSawyer
{
    void Interact();
    void OnTouchingPlayer();
    void OnNotTouchingPlayer();

}
public class InteractSawyer : MonoBehaviour
{
    private IInteractableSawyer currentInteractable;
    public GameObject interactUI;
    public TextMeshProUGUI interactionText;
    public bool touching;
    void Start()
    {
        touching = false;
    }
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact();
            currentInteractable = null;
            interactUI.SetActive(false);
        }

        if (touching == false) interactUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        IInteractableSawyer interactable = collision.GetComponent<IInteractableSawyer>();

        if (interactable != null)
        {
            touching = true;
            interactUI.SetActive(true);
            currentInteractable = interactable;
            currentInteractable.OnTouchingPlayer();


        }
    }
    private void OnTriggerExit(Collider collision)
    {
        IInteractableSawyer interactable = collision.GetComponent<IInteractableSawyer>();

        if (interactable != null && interactable == currentInteractable)
        {
            interactUI.SetActive(false);

            currentInteractable.OnNotTouchingPlayer();
            currentInteractable = null;
            touching = false;

        }
    }
}