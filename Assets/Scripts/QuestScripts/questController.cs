using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance;

    public List<Quest.QuestProgress> activeQuests =
        new List<Quest.QuestProgress>();

    [Header("All Quests")]
    public Quest[] quests;

    public QuestLogController questLogUI;

    private void Awake()
    {
        Instance = this;
    }

    public Quest GetQuestFromTask(string taskID)
    {
        foreach (Quest quest in quests)
        {
            if (quest.taskID == taskID)
            {
                return quest;
            }
        }

        return null;
    }

    public void StartQuest(Quest quest, QuestGiver questGiver)
    {
        // Check if this NPC already gave the player this quest.
        foreach (Quest.QuestProgress progress in activeQuests)
        {
            if (progress.questGiver == questGiver &&
                progress.quest == quest)
            {
                Debug.Log(
                    "This NPC already gave you this quest."
                );

                return;
            }
        }

        Quest.QuestProgress newProgress =
            new Quest.QuestProgress(quest, questGiver);

        activeQuests.Add(newProgress);

        Debug.Log(
            "Started quest: " +
            quest.questName +
            " from " +
            questGiver.name
        );

        if (!newProgress.IsCompleted &&
            newProgress.CurrentObjective.type ==
            Quest.objectiveType.FirstTalk)
        {
            newProgress.currentObjectiveIndex++;
        }

        UpdateQuestLog();
    }

    public bool CanCompleteObjective(
        Quest.objectiveType type,
        string gameID)
    {
        foreach (Quest.QuestProgress progress in activeQuests)
        {
            if (progress.IsCompleted)
                continue;

            Quest.QuestObjective objective =
                progress.CurrentObjective;

            if (objective.type == type &&
                objective.gameID == gameID)
            {
                return true;
            }
        }

        return false;
    }

    public void CompleteMicrogame(string gameID)
    {
        foreach (Quest.QuestProgress progress in activeQuests)
        {
            if (progress.IsCompleted)
                continue;

            Quest.QuestObjective objective =
                progress.CurrentObjective;

            if (objective.type == Quest.objectiveType.Microgame &&
                objective.gameID == gameID)
            {
                Debug.Log(
                    "Completed microgame objective: " +
                    objective.description
                );

                progress.currentObjectiveIndex++;

                UpdateQuestLog();

                return;
            }
        }
    }

    public bool ReturnToNPC(QuestGiver questGiver)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            Quest.QuestProgress progress = activeQuests[i];

            if (progress.IsCompleted)
                continue;

            if (progress.questGiver != questGiver)
                continue;

            if (progress.CurrentObjective.type ==
                Quest.objectiveType.ReturnToNPC)
            {
                Debug.Log(
                    "Completed quest: " +
                    progress.quest.questName
                );

                activeQuests.RemoveAt(i);

                UpdateQuestLog();

                return true;
            }
        }

        return false;
    }

    private void UpdateQuestLog()
    {
        if (questLogUI != null)
        {
            questLogUI.UpdateQuestLog();
        }
    }
}