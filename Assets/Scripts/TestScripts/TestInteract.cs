using UnityEngine;

public class TestInteract : MonoBehaviour, IInteractable
{
    public GameObject interactionUI;

    public void Interact()
    {
        Destroy(gameObject);
        interactionUI.SetActive(false);

    }

    public void OnNotTouchingPlayer()
    {

    }

    public void OnTouchingPlayer()
    {

    }
}
