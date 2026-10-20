using UnityEngine;

public class QuestGiver : MonoBehaviour, IInteractable
{
    public ChairScript chair;
    public DialogInteraction text;
    public string questid;

    private void Awake()
    {
        if (chair == null)
            chair = GetComponentInParent<ChairScript>();
    }

    public void Interact()
    {
        QuestController controller = QuestController.Instance;

        if (controller != null && controller.ReturnToNPC(this))
            return;

        if (text == null)
        {
            Debug.LogWarning("QuestGiver is missing its DialogInteraction reference.", this);
            return;
        }

        text.StartDiologue();
    }

    public void GiveQuest(string questid)
    {
        QuestController controller = QuestController.Instance;
        if (controller == null)
        {
            Debug.LogError("Cannot give a quest because no QuestController exists.", this);
            return;
        }

        Quest quest = controller.GetQuestFromTask(questid);
        if (quest == null)
        {
            Debug.LogWarning("No quest found for task: " + questid, this);
            return;
        }

        if (controller.IsQuestActive(quest, this))
        {
            controller.StartQuest(quest, this);
            return;
        }

        PrepareSpecialObjective(quest);
        controller.StartQuest(quest, this);
    }

    private void PrepareSpecialObjective(Quest quest)
    {
        if (quest.objectives == null)
            return;

        foreach (Quest.QuestObjective objective in quest.objectives)
        {
            if (objective == null || objective.gameID != "BookRequestGame")
                continue;

            BookRequestGame bookGame = FindFirstObjectByType<BookRequestGame>(FindObjectsInactive.Include);
            if (bookGame != null)
            {
                bookGame.PrepareRequest();
            }
            else
            {
                Debug.LogError("BookRequestGame not found.", this);
            }

            return;
        }
    }

    public void OnTouchingPlayer() { }
    public void OnNotTouchingPlayer() { }
}
