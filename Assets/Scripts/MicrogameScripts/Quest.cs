using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Quests/Quest:")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public string taskID;
    public List<QuestObjective> objectives;
    public enum objectiveType {FirstTalk, Microgame, ReturnToNPC}


    //called when scriptable object is edited
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }
    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveID; //match this with item ID that you need to collect, microgame to complete, etc
        public string description;
        public objectiveType type;

        public bool isCompleted;
    }

    [System.Serializable]
    public class QuestProgress
    {
        public Quest quest;

        // Which objective the player is currently doing
        public int currentObjectiveIndex;

        public QuestProgress(Quest quest)
        {
            this.quest = quest;
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
            get
            {
                return currentObjectiveIndex >= quest.objectives.Count;
            }
        }

        public string QuestID => quest.questID;
    }
}