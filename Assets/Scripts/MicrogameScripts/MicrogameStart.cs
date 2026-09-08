using UnityEngine;

public class MicroGameStart : MonoBehaviour, IInteractable
{
    [Header("Quest ID")]
    [Tooltip("Must match the MiniGame objective ID in the Quest asset.")]
    public string taskID;

    [Header("Minigame")]
    [Tooltip("The minigame this location starts.")]
    public string minigameID;
    [Tooltip("The UI's for each game.")]

    public GameObject ButtonGame1UI;
    public GameObject ButtonGame2UI;
    public GameObject ButtonGame3UI;


    [Header("Interaction")]
    public bool interacted = false;

    [Header("Timer Stuff")]
    public GameTimer timerScript;
    public GameObject TimerUI;

    void Start()
    {
        ButtonGame1UI = FindInactiveObject("ButtonGame1UI");
        ButtonGame2UI = FindInactiveObject("ButtonGame2UI");
        ButtonGame3UI = FindInactiveObject("ButtonGame3UI");


        TimerUI = FindInactiveObject("GameTimer");
        timerScript = TimerUI.GetComponent<GameTimer>();
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
        // Only allow interaction if this location is the current objective of an active quest.
        if (!IsValidQuestTask())
        {
            Debug.Log("This task is not currently required by a quest.");
            return;
        }

        if (interacted)
            return;

        interacted = true;

        StartMinigame();
    }

    private bool IsValidQuestTask()
    {
        return QuestController.Instance.CanCompleteObjective(
            Quest.objectiveType.Microgame,
            taskID
        );
    }

    private void StartMinigame()
    {
        Debug.Log("Starting minigame: " + minigameID);

        switch (minigameID)
        {
            case "ButtonGame1":
                TimerUI.SetActive(true);

                ButtonGame1UI.SetActive(true);
                ButtonGame1UI.GetComponent<ButtonGame>().StartGame();
                timerScript.sliderTimer = ButtonGame1UI.GetComponent<ButtonGame>().TimerTime;
                break;

            case "ButtonGame2":
                // Start ButtonGame2
                break;

            case "ButtonGame3":
                // Start ButtonGame3
                break;

            default:
                Debug.LogWarning(
                    "No minigame found for ID: " + minigameID
                );
                break;
        }
    }

    public void OnTouchingPlayer()
    {

    }

    public void OnNotTouchingPlayer()
    {

    }
}