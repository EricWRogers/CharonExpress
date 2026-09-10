using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance;

    public List<Quest.QuestProgress> activeQuests =
        new List<Quest.QuestProgress>();

    private void Awake()
    {
        Instance = this;
    }

    public void StartQuest(Quest quest)
    {
        // Don't start the same quest twice
        if (HasQuest(quest.questID))
        {
            Debug.Log("Already have quest: " + quest.questName);
            return;
        }

        Quest.QuestProgress progress =
            new Quest.QuestProgress(quest);

        activeQuests.Add(progress);

        Debug.Log("Started quest: " + quest.questName);
    }

    public bool HasQuest(string questID)
    {
        foreach (var progress in activeQuests)
        {
            if (progress.QuestID == questID)
                return true;
        }

        return false;
    }

    public bool CanCompleteObjective(
        Quest.objectiveType type,
        string objectiveID)
    {
        foreach (var progress in activeQuests)
        {
            if (progress.IsCompleted)
                continue;

            Quest.QuestObjective objective =
                progress.CurrentObjective;

            if (objective.type == type &&
                objective.objectiveID == objectiveID)
            {
                return true;
            }
        }

        return false;
    }

    public void CompleteObjective(
        Quest.objectiveType type,
        string objectiveID)
    {
        foreach (var progress in activeQuests)
        {
            if (progress.IsCompleted)
                continue;

            Quest.QuestObjective objective =
                progress.CurrentObjective;

            if (objective.type == type &&
                objective.objectiveID == objectiveID)
            {
                Debug.Log(
                    "Completed objective: " +
                    objective.description
                );

                progress.currentObjectiveIndex++;

                if (progress.IsCompleted)
                {
                    Debug.Log(
                        "QUEST COMPLETE: " +
                        progress.quest.questName
                    );
                }

                return;
            }
        }
    }
}