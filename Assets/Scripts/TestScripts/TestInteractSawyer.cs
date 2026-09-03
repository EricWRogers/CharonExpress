using UnityEngine;

public class TestInteractSawyer : MonoBehaviour, IInteractableSawyer
{
    public void Interact()
    {
        gameObject.SetActive(false);
    }

    public void OnNotTouchingPlayer()
    {

    }

    public void OnTouchingPlayer()
    {

    }
}
