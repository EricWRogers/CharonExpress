using UnityEngine;

public class QuestGiver : MonoBehaviour, IInteractable
{
    public ChairScript chair;
    public DialogInteraction text;
    [Header("Quest")]
    public Quest questToGive;
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

    public void GiveQuest(Quest quest)
    {
        QuestController controller = QuestController.Instance;

        if (controller == null)
        {
            Debug.LogError(
                "Cannot give a quest because no QuestController exists.",
                this);
            return;
        }

        if (quest == null)
        {
            Debug.LogWarning(
                "The dialogue has no Quest asset assigned.",
                this);
            return;
        }

        controller.StartQuest(quest, this);
    }
    public void OnTouchingPlayer() { }
    public void OnNotTouchingPlayer() { }
}
