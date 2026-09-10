using UnityEngine;

public class QuestGiver : MonoBehaviour, IInteractable
{
    public ChairScript chair;
    public DialogInteraction text;

    // Put the quests this NPC can give here.
    public Quest[] quests;

    // Unique ID for this NPC.
    public string npcID;
    void Start()
    {
        ChairScript chairScript = GetComponentInParent<ChairScript>();
    }

    public void Interact()
    {
        // see if this NPC is the return point for one of the player's active quests.
        if (QuestController.Instance.CanCompleteObjective(
            Quest.objectiveType.ReturnToNPC,
            npcID))
        {
            QuestController.Instance.CompleteObjective(
                Quest.objectiveType.ReturnToNPC,
                npcID
            );

            return;
        }

        // call the dialogue stuff
        //text.StartDiologue();
        // give a new quest based on this NPC's chair task.
        Quest quest = GetQuestFromChair();

        if (quest == null)
        {
            Debug.Log(
                "No quest found for task: " +
                chair.task
            );

            return;
        }

        QuestController.Instance.StartQuest(quest);

        // Complete the talking objective immediately
        QuestController.Instance.CompleteObjective(
            Quest.objectiveType.FirstTalk,
            npcID
        );
    }

    private Quest GetQuestFromChair()
    {
        foreach (Quest quest in quests)
        {
            if (quest.taskID == chair.task)
            {
                return quest;
            }
        }

        return null;
    }

    public void OnTouchingPlayer()
    {
    }

    public void OnNotTouchingPlayer()
    {
    }
}