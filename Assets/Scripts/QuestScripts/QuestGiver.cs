using UnityEngine;

/// <summary>
/// Handles player interaction with an NPC that can assign quests
/// </summary>
public class QuestGiver : MonoBehaviour, IInteractable
{
    [Header("Quest Giver")]
    [Tooltip("The chair associated with this quest giver.")]
    public ChairScript chair;

    [Tooltip("The dialogue interaction component used to start conversations with this NPC.")]
    public DialogInteraction text;

    [Header("Quest")]
    [Tooltip("The quest asset that can be assigned by this NPC.")]
    public Quest questToGive;

    private void Awake()
    {
        if (chair == null)
            chair = GetComponentInParent<ChairScript>();
    }

    /// <summary>
    /// Handles player interaction by first checking for a return-to-NPC objective.
    /// If no matching objective is completed, starts the NPC's dialogue
    /// </summary>
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

    /// <summary>
    /// Starts the specified quest through the QuestController.
    /// </summary>
    /// <param name="quest">The quest to assign to the player.</param>
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