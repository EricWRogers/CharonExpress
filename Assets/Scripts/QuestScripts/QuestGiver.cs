using JetBrains.Annotations;
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
        // Check if this NPC gave the player a quest that is now ready to be turned in.
        if (QuestController.Instance.ReturnToNPC(this))
        {
            return;
        }
        
        // call the dialogue stuff
        text.StartDiologue();

        // Get the quest that matches the task assigned to this NPC's chair.
        /* Quest quest =
            QuestController.Instance.GetQuestFromTask(questid);

        if (quest == null)
        {
            Debug.Log(
                "No quest found for task: " +
                questid
            );

            return;
        }
        Debug.Log(quest);

        // Start a new copy of the quest and remember this exact NPC as the NPC that gave the quest.
        QuestController.Instance.StartQuest(
            quest,
            this
        );*/
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
        Debug.Log(quest);

        // Start a new copy of the quest and remember this exact NPC as the NPC that gave the quest.
        QuestController.Instance.StartQuest(
            quest,
            this
        );
    }

    public void OnTouchingPlayer() { }
    public void OnNotTouchingPlayer() {}
}