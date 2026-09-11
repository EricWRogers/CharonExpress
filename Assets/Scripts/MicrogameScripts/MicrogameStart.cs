using UnityEngine;

public class MicroGameStart : MonoBehaviour, IInteractable
{
    [Header("Quest ID")]
    [Tooltip("Must match the Microgame game ID in the Quest asset.")]
    public string gameID;

    [Header("Minigame")]
    [Tooltip("The UI's for each game.")]

    public GameObject ButtonGame1UI;
    public GameObject ButtonGame2UI;
    public GameObject ButtonGame3UI;

    [Header("Interaction")]
    public bool interacted = false;

    [Header("Timer Stuff")]
    public GameObject TimerUI;

    void Start()
    {
        ButtonGame1UI = FindInactiveObject("ButtonGame1UI");
        ButtonGame2UI = FindInactiveObject("ButtonGame2UI");
        ButtonGame3UI = FindInactiveObject("ButtonGame3UI");

        TimerUI = FindInactiveObject("GameTimer");
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
            gameID
        );
    }

    private void StartMinigame()
    {
        Debug.Log("Starting minigame: " + gameID);

        switch (gameID)
        {
            case "ButtonGame1":
                TimerUI.SetActive(true);

                ButtonGame1UI.SetActive(true);
                ButtonGame1UI.GetComponent<ButtonGame>().StartGame();
                break;

            case "ButtonGame2":
                TimerUI.SetActive(true);

                ButtonGame2UI.SetActive(true);
                ButtonGame2UI.GetComponent<ButtonGame2>().StartGame();
                break;

            case "ButtonGame3":
                // Start ButtonGame3
                break;

            default:
                Debug.LogWarning(
                    "No minigame found for ID: " + gameID
                );
                break;
        }
    }

    public void ResetInteraction()
    {
        interacted = false;
    }

    public void OnTouchingPlayer() {}
    public void OnNotTouchingPlayer() {}
}