using UnityEngine;

public class QuestGiver : MonoBehaviour, IInteractable
{
    public ChairScript chair;
    public DialogInteraction text;

    void Start()
    {
        chair = GetComponentInParent<ChairScript>();
    }
    public void Interact()
    {
        // Check if this NPC gave the player a quest that is now ready to be turned in.
        if (QuestController.Instance.ReturnToNPC(this))
        {
            return;
        }
        
        // call the dialogue stuff
        //text.StartDiologue();

        // Get the quest that matches the task assigned to this NPC's chair.
        Quest quest =
            QuestController.Instance.GetQuestFromTask(chair.task);

        if (quest == null)
        {
            Debug.Log(
                "No quest found for task: " +
                chair.task
            );

            return;
        }

        // Start a new copy of the quest and remember this exact NPC as the NPC that gave the quest.
        QuestController.Instance.StartQuest(
            quest,
            this
        );
    }

    public void OnTouchingPlayer() {}
    public void OnNotTouchingPlayer() {}
}