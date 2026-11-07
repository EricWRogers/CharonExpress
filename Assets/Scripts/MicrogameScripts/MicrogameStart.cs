using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroGameStart : MonoBehaviour, IInteractable
{
    [Header("Quest ID")]
    [Tooltip("Must match the Microgame game ID in the Quest asset.")]
    public MicrogameId gameID;

    [Header("Minigame")]
    [Tooltip("The UI's for each game.")]

    public GameObject FallingCatchUI;
    public GameObject TypingUI;
    public GameObject BookRequestUI;

    [Header("Interaction")]
    public bool interacted = false;

    [Header("Timer Stuff")]
    public GameObject TimerUI;
    public GameObject player;

    void Start()
    {
        FallingCatchUI = FindInactiveObject("FallingCatchUI");
        TypingUI = FindInactiveObject("TypingUI");
        BookRequestUI = FindInactiveObject("BookRequestUI");

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
        player.GetComponent<AudioSource>().PlayOneShot(player.GetComponent<player>().bubbleSound);
        if (!IsValidQuestTask())
        {
            Debug.Log("This task is not currently required by a quest." + gameID);
            return;
        }

        if (interacted)
            return;

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
            case MicrogameId.FallingCatchGame:
                StartCoroutine(RunFunctionForFiveSeconds());
                FallingCatchUI.SetActive(true);
                FallingCatchUI.GetComponent<FallingCatchGame>().StartGame();
                break;

            case MicrogameId.BookRequestGame:
                StartCoroutine(RunFunctionForFiveSeconds());
                BookRequestUI.SetActive(true);
                BookRequestUI.GetComponent<BookRequestGame>().StartGame();
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

    IEnumerator RunFunctionForFiveSeconds()
    {
        float elapsedTime = 0f;
        float duration = 0.1f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void OnTouchingPlayer() { }
    public void OnNotTouchingPlayer() {}
}