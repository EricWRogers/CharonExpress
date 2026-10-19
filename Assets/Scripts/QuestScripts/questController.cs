using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance;

    [Header("Quest State")]
    public List<Quest.QuestProgress> activeQuests = new List<Quest.QuestProgress>();

    [Header("All Quests")]
    public Quest[] quests;

    public QuestLogController questLogUI;

    [Header("Quest Completion")]
    [Tooltip("Invoked whenever any quest is completed, regardless of which objective path completed it.")]
    public UnityEvent onAnyQuestCompleted = new UnityEvent();

    public event Action<Quest, QuestGiver> QuestCompleted;
    
    public DayCycle dayCycle;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one QuestController exists. The duplicate will be removed.", this);
            Destroy(this);
            return;
        }

        Instance = this;

        if (activeQuests == null)
            activeQuests = new List<Quest.QuestProgress>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public Quest GetQuestFromTask(string taskID)
    {
        if (string.IsNullOrWhiteSpace(taskID) || quests == null)
            return null;

        foreach (Quest quest in quests)
        {
            if (quest != null && quest.taskID == taskID)
                return quest;
        }

        return null;
    }

    public bool IsQuestActive(Quest quest, QuestGiver questGiver)
    {
        if (quest == null || activeQuests == null)
            return false;

        foreach (Quest.QuestProgress progress in activeQuests)
        {
            if (progress != null &&
                progress.quest == quest &&
                progress.questGiver == questGiver)
            {
                return true;
            }
        }

        return false;
    }

    public void StartQuest(Quest quest, QuestGiver questGiver)
    {
        if (quest == null)
        {
            Debug.LogWarning("Cannot start a quest because the quest reference is missing.", this);
            return;
        }

        if (activeQuests == null)
            activeQuests = new List<Quest.QuestProgress>();

        if (IsQuestActive(quest, questGiver))
        {
            Debug.Log("This NPC already gave you this quest.");
            return;
        }

        Quest.QuestProgress progress = new Quest.QuestProgress(quest, questGiver);
        activeQuests.Add(progress);

        string giverName = questGiver != null ? questGiver.name : "Unknown NPC";
        Debug.Log("Started quest: " + quest.questName + " from " + giverName);

        Quest.QuestObjective objective = progress.CurrentObjective;
        if (objective != null && objective.type == Quest.objectiveType.FirstTalk)
        {
            CompleteCurrentObjective(progress);
            return;
        }

        if (progress.IsCompleted)
            CompleteQuest(progress);
        else
            UpdateQuestLog();
    }

    public bool CanCompleteObjective(Quest.objectiveType type, string gameID)
    {
        if (activeQuests == null)
            return false;

        foreach (Quest.QuestProgress progress in activeQuests)
        {
            if (progress == null || progress.IsCompleted)
                continue;

            if (MatchesCurrentObjective(progress, type, gameID))
                return true;
        }

        return false;
    }

    public void CompleteMicrogame(string gameID)
    {
        if (activeQuests == null)
            return;

        for (int i = 0; i < activeQuests.Count; i++)
        {
            Quest.QuestProgress progress = activeQuests[i];
            if (progress == null || progress.IsCompleted)
                continue;

            if (!MatchesCurrentObjective(progress, Quest.objectiveType.Microgame, gameID))
                continue;

            Quest.QuestObjective objective = progress.CurrentObjective;
            Debug.Log("Completed microgame objective: " + objective.description);
            CompleteCurrentObjective(progress);
            return;
        }
    }

    public bool FailQuest(string gameID)
    {
        if (activeQuests == null)
            return false;

        for (int i = 0; i < activeQuests.Count; i++)
        {
            Quest.QuestProgress progress = activeQuests[i];
            if (progress == null || progress.IsCompleted)
                continue;

            if (!MatchesCurrentObjective(progress, Quest.objectiveType.Microgame, gameID))
                continue;

            Debug.Log("Failed quest: " + progress.quest.questName);
            activeQuests.RemoveAt(i);
            UpdateQuestLog();
            return true;
        }

        return false;
    }

    public bool ReturnToNPC(QuestGiver questGiver)
    {
        if (activeQuests == null)
            return false;

        for (int i = 0; i < activeQuests.Count; i++)
        {
            Quest.QuestProgress progress = activeQuests[i];
            if (progress == null || progress.IsCompleted)
                continue;

            if (progress.questGiver != questGiver)
                continue;

            Quest.QuestObjective objective = progress.CurrentObjective;
            if (objective == null || objective.type != Quest.objectiveType.ReturnToNPC)
                continue;

            Debug.Log("Completed return-to-NPC objective: " + objective.description);
            CompleteCurrentObjective(progress);
            return true;
        }

        return false;
    }

    private void CompleteCurrentObjective(Quest.QuestProgress progress)
    {
        if (progress == null || progress.IsCompleted)
            return;

        Quest.QuestObjective completedObjective = progress.CurrentObjective;
        if (completedObjective == null)
            return;

        progress.currentObjectiveIndex++;

        // An objective can end the quest early, or the quest ends when its final objective is done.
        if (completedObjective.completeQuestAfterThis || progress.IsCompleted)
            CompleteQuest(progress);
        else
            UpdateQuestLog();
    }

    private void CompleteQuest(Quest.QuestProgress progress)
    {
        if (progress == null || progress.quest == null || activeQuests == null)
            return;

        if (!activeQuests.Remove(progress))
            return;

        Quest completedQuest = progress.quest;
        QuestGiver questGiver = progress.questGiver;

        Debug.Log("Completed quest: " + completedQuest.questName, this);

        // test for adding points
        dayCycle.AddPoints(1);


        if (onAnyQuestCompleted != null)
            onAnyQuestCompleted.Invoke();

        QuestCompleted?.Invoke(completedQuest, questGiver);
        UpdateQuestLog();
    }

    private bool MatchesCurrentObjective(
        Quest.QuestProgress progress,
        Quest.objectiveType type,
        string gameID)
    {
        Quest.QuestObjective objective = progress.CurrentObjective;

        return objective != null &&
               objective.type == type &&
               objective.gameID == gameID;
    }


    public void ClearAllQuests()
    {
        activeQuests.Clear();

        UpdateQuestLog();

        Debug.Log("All active quests cleared at station arrival.");
    }

    private void UpdateQuestLog()
    {
        if (questLogUI != null)
            questLogUI.UpdateQuestLog();
    }
}
