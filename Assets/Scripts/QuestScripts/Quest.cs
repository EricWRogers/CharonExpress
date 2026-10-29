using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest")]
    [Tooltip("The name displayed for this quest.")]
    public string questName;

    [Tooltip("The description of the quest.")]
    [TextArea]
    public string description;

    public bool unfailable = false;


    [Header("Objectives")]
    [Tooltip("The objectives that must be completed to finish this quest.")]
    public List<QuestObjective> objectives = new List<QuestObjective>();

    /// <summary>
    /// Defines the types of objectives that can be used in a quest.
    /// </summary>
    public enum objectiveType
    {
        FirstTalk,
        Microgame,
        ReturnToNPC
    }

    /// <summary>
    /// Stores the data required to define a single quest objective.
    /// </summary>
    [Serializable]
    public class QuestObjective
    {
        [Tooltip("Identifier used by the gameplay system for this objective.")]
        public MicrogameId gameID;

        [Tooltip("The description displayed for this objective.")]
        [TextArea]
        public string description;

        [Tooltip("The type of action required to complete this objective.")]
        public objectiveType type;

        [Tooltip("End the quest immediately after this objective is completed, even if more objectives follow.")]
        public bool completeQuestAfterThis;
    }

    /// <summary>
    /// Stores the runtime progress of an active quest, including its quest giver,
    /// current objective, and any additional book request information.
    /// </summary>
    [Serializable]
    public class QuestProgress
    {
        [Tooltip("The quest asset associated with this progress.")]
        public Quest quest;

        [Tooltip("The NPC that assigned this quest.")]
        public QuestGiver questGiver;

        [Tooltip("The index of the current objective in the quest's objective list.")]
        [Min(0)]
        public int currentObjectiveIndex;

        [Tooltip("The subject requested for the book task.")]
        public string requestedBookSubject;

        [Tooltip("The color requested for the book task.")]
        public string requestedBookColor;

        /// <summary>
        /// Initializes quest progress for the specified quest and quest giver.
        /// Sets the current objective index to zero.
        /// </summary>
        /// <param name="quest">The quest whose progress will be tracked.</param>
        /// <param name="questGiver">The NPC that assigned the quest.</param>
        public QuestProgress(Quest quest, QuestGiver questGiver)
        {
            this.quest = quest;
            this.questGiver = questGiver;
            currentObjectiveIndex = 0;
        }

        /// <summary>
        /// Gets the current objective of the quest.
        /// </summary>
        public QuestObjective CurrentObjective
        {
            get
            {
                if (quest == null || quest.objectives == null)
                    return null;

                if (currentObjectiveIndex < 0 || currentObjectiveIndex >= quest.objectives.Count)
                    return null;

                return quest.objectives[currentObjectiveIndex];
            }
        }

        /// <summary>
        /// Gets whether the quest has no current objective remaining.
        /// </summary>
        public bool IsCompleted
        {
            get { return CurrentObjective == null; }
        }
    }
}