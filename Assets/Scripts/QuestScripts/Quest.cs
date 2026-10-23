using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest")]
    public string questName;

    [TextArea]
    public string description;


    [Header("Objectives")]
    public List<QuestObjective> objectives = new List<QuestObjective>();

    public enum objectiveType
    {
        FirstTalk,
        Microgame,
        ReturnToNPC
    }

    [Serializable]
    public class QuestObjective
    {
        [Tooltip("Identifier used by the gameplay system for this objective.")]
        public MicrogameId gameID;

        [TextArea]
        public string description;

        public objectiveType type;

        [Tooltip("End the quest immediately after this objective is completed, even if more objectives follow.")]
        public bool completeQuestAfterThis;
    }

    [Serializable]
    public class QuestProgress
    {
        public Quest quest;
        public QuestGiver questGiver;

        [Min(0)]
        public int currentObjectiveIndex;
        public string requestedBookSubject;
        public string requestedBookColor;

        public QuestProgress(Quest quest, QuestGiver questGiver)
        {
            this.quest = quest;
            this.questGiver = questGiver;
            currentObjectiveIndex = 0;
        }

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

        public bool IsCompleted
        {
            get { return CurrentObjective == null; }
        }
    }
}
