using UnityEngine;

public class QuestGiver : MonoBehaviour, IInteractable
{
    public ChairScript chair;
    public DialogInteraction text;
    public string questid;

    void Start()
    {
        chair = GetComponentInParent<ChairScript>();
    }

    public void Interact()
    {
        // Try to turn in the quest first.
        if (QuestController.Instance.ReturnToNPC(this))
        {
            return;
        }

        // Start the dialogue.
        text.StartDiologue();
    }

    public void GiveQuest(string questid)
    {
        Quest quest =
            QuestController.Instance.GetQuestFromTask(questid);

        if (quest == null)
        {
            Debug.Log(
                "No quest found for task: " +
                questid
            );

            return;
        }

        foreach (Quest.QuestObjective objective in quest.objectives)
        {
            if (objective.gameID == "BookRequestGame")
            {
                BookRequestGame bookGame =
                    FindFirstObjectByType<BookRequestGame>(
                        FindObjectsInactive.Include
                    );

                if (bookGame != null)
                {
                    bookGame.PrepareRequest();
                }
                else
                {
                    Debug.LogError(
                        "BookRequestGame not found."
                    );
                }

                break;
            }
        }

        // Now give the quest.
        QuestController.Instance.StartQuest(
            quest,
            this
        );
    }

    public void OnTouchingPlayer()
    {
    }

    public void OnNotTouchingPlayer()
    {
    }
}