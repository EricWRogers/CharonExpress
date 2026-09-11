using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questName;

    [TextArea]
    public string description;

    [Tooltip("Must match the task value from ChairScript.")]
    public string taskID;

    public List<QuestObjective> objectives;

    public enum objectiveType
    {
        FirstTalk,
        Microgame,
        ReturnToNPC
    }

    [Serializable]
    public class QuestObjective
    {
        public string gameID;
        public string description;
        public objectiveType type;
    }

    [Serializable]
    public class QuestProgress
    {
        public Quest quest;
        public QuestGiver questGiver;
        public int currentObjectiveIndex;

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
                if (currentObjectiveIndex >= quest.objectives.Count)
                    return null;

                return quest.objectives[currentObjectiveIndex];
            }
        }

        public bool IsCompleted
        {
            get { return currentObjectiveIndex >= quest.objectives.Count; }
        }
    }
}