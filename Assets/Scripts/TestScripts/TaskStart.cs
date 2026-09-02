using UnityEngine;

public class TaskStart : MonoBehaviour, IInteractable
{
    public ButtonGame targetscript;
    public GameTimer timerScript;
    public GameObject GameUI;
    public GameObject TimerUI;
    public bool interacted = false;
    
    public void Interact()
    {
        if (!interacted)
        {
            GameUI.SetActive(true);
            targetscript.StartGame();
            
            TimerUI.SetActive(true);
            timerScript.StartGameTimer();

            //interacted = true;
            //disabled for testing purposes
        }


    }

    public void OnNotTouchingPlayer()
    {
        
    }

    public void OnTouchingPlayer()
    {

    }
}
