using UnityEngine;

public class TaskStart : MonoBehaviour, IInteractable
{
    public GameTimer timerScript;
    public GameObject TimerUI;

    public GameObject ButtonGame1UI;
    public GameObject ButtonGame2UI;
    public GameObject ButtonGame3UI;


    public bool interacted = false;
    void Start()
    {
        ButtonGame1UI = FindInactiveObject("ButtonGame1UI");
        ButtonGame2UI = FindInactiveObject("ButtonGame2UI");
        ButtonGame3UI = FindInactiveObject("ButtonGame3UI");
        

        TimerUI = FindInactiveObject("GameTimer");
        timerScript = TimerUI.GetComponent<GameTimer>();
    }
    void OnEnable()
    {
        interacted = false;
    }

    GameObject FindInactiveObject(string name)
    {
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in objects)
        {
            if (obj.name == name && obj.scene.IsValid())
            {
                return obj;
            }
        }

        return null;
    }
    
    public void Interact()
    {
        if (!interacted)
        {
            TimerUI.SetActive(true);

            ChairScript chairScript = GetComponentInParent<ChairScript>();
            switch (chairScript.task)
            {
                case "Hit button 1":
                    ButtonGame1UI.SetActive(true);
                    ButtonGame1UI.GetComponent<ButtonGame>().StartGame();
                    timerScript.sliderTimer = ButtonGame1UI.GetComponent<ButtonGame>().TimerTime;
                    break;

                case "Hit button 2":
                    ButtonGame2UI.SetActive(true);
                    ButtonGame2UI.GetComponent<ButtonGame2>().StartGame();
                    timerScript.sliderTimer = ButtonGame2UI.GetComponent<ButtonGame2>().TimerTime;
                    break;

                case "Hit button 3":
                    ButtonGame3UI.SetActive(true);
                    ButtonGame3UI.GetComponent<ButtonGame3>().StartGame();
                    timerScript.sliderTimer = ButtonGame3UI.GetComponent<ButtonGame3>().TimerTime;
                    break;
                
                case "":
                    Debug.Log("No task assigned to this chair.");
                    break;
                case "Interact":
                    Debug.Log("you removed the task .");
                    break;
            }
            timerScript.StartGameTimer();

            interacted = true;
        }
    }

    public void OnNotTouchingPlayer()
    {
        
    }

    public void OnTouchingPlayer()
    {

    }
}
