using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages active quests, objective completion, quest failures,
/// quest rewards, and quest log updates.
/// </summary>
public class QuestController : MonoBehaviour
{
    public static QuestController Instance;

    [Header("Quest State")]
    [Tooltip("The list of quests currently being tracked.")]
    public List<Quest.QuestProgress> activeQuests = new List<Quest.QuestProgress>();

    [Header("Quest Log UI")]
    [Tooltip("The controller responsible for displaying and updating the quest log.")]
    public QuestLogController questLogUI;

    [Header("Quest Completion")]
    [Tooltip("Invoked whenever any quest is completed.")]
    public UnityEvent onAnyQuestCompleted = new UnityEvent();

    /// <summary>
    /// Event invoked when a quest is completed.
    /// Provides the completed quest and the NPC that assigned it.
    /// </summary>
    public event Action<Quest, QuestGiver> QuestCompleted;

    [Header("Day Cycle")]
    [Tooltip("The day cycle controller script.")]
    public DayCycle dayCycle;

    [Tooltip("The end-of-day UI controller script.")]
    public EndDayUI endDayUI;

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

    /// <summary>
    /// Checks whether the specified quest is active for the specified quest giver.
    /// </summary>
    /// <param name="quest">The quest to check.</param>
    /// <param name="questGiver">The NPC associated with the quest.</param>
    /// <returns>True if the quest is active for the specified NPC; otherwise, false.</returns>
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

    /// <summary>
    /// Starts tracking the specified quest for the given quest giver.
    /// Prevents duplicate active quests from the same NPC, assigns the quest
    /// to the associated ghost when available, prepares book request task,
    /// and processes the initial objective.
    /// </summary>
    /// <param name="quest">The quest to start.</param>
    /// <param name="questGiver">The NPC assigning the quest.</param>
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


        GhostScript ghost = questGiver.GetComponentInParent<GhostScript>();

        if (ghost != null)
        {
            ghost.AssignQuest(progress);
        }

        PrepareBookRequest(progress);

        questGiver.transform.parent.GetComponent<GhostScript>().EnableHighlight();

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

    /// <summary>
    /// Checks whether an objective of the specified type and microgame ID
    /// can be completed by an active quest.
    /// </summary>
    /// <param name="type">The type of objective to check.</param>
    /// <param name="gameID">The microgame ID associated with the objective.</param>
    /// <returns>True if a matching objective can be completed; otherwise, false.</returns>
    public bool CanCompleteObjective(
        Quest.objectiveType type,
        MicrogameId gameID)
    {
        if (type == Quest.objectiveType.Microgame)
            return GetNextMicrogameProgress(gameID) != null;

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

    /// <summary>
    /// Completes the next active quest objective associated with the specified microgame ID,
    /// if a matching quest objective is available.
    /// </summary>
    /// <param name="gameID">The ID of the completed microgame.</param>
    public void CompleteMicrogame(MicrogameId gameID)
    {
        CompleteMicrogame(GetNextMicrogameProgress(gameID));
    }

    /// <summary>
    /// Completes the current microgame objective for the specified quest progress
    /// if the progress belongs to an active, incomplete quest
    /// and its current objective is a microgame objective.
    /// </summary>
    /// <param name="progress">The active quest progress whose objective should be completed.</param>
    public void CompleteMicrogame(Quest.QuestProgress progress)
    {
        if (progress == null ||
            activeQuests == null ||
            !activeQuests.Contains(progress) ||
            progress.IsCompleted)
        {
            return;
        }

        Quest.QuestObjective objective = progress.CurrentObjective;

        if (objective == null ||
            objective.type != Quest.objectiveType.Microgame)
        {
            return;
        }

        Debug.Log("Completed microgame objective: " + objective.description);
        CompleteCurrentObjective(progress);
    }

    /// <summary>
    /// Fails the specified active quest, updates failure tracking and the quest log,
    /// and optionally reassigns the associated ghost.
    /// </summary>
    /// <param name="progress">The quest progress to fail.</param>
    /// <param name="reassignGhost">Whether to reassign the associated ghost after the quest fails.</param>
    /// <returns>True if the quest was successfully failed; otherwise, false.</returns>
    /// To use: QuestController.Instance.FailQuest(QuestController.Instance.GetNextMicrogameProgress(gameID), false);

    public bool FailQuest(
        Quest.QuestProgress progress,
        bool reassignGhost = true)
    {
        if (progress == null ||
            activeQuests == null ||
            !activeQuests.Contains(progress) ||
            progress.IsCompleted)
        {
            return false;
        }


        Debug.Log("Failed quest: " + progress.quest.questName);

        endDayUI.failedQuests++;

        GhostScript ghost = null;

        if (progress.questGiver != null &&
            progress.questGiver.transform.parent != null)
        {
            ghost = progress.questGiver.transform.parent
                .GetComponent<GhostScript>();
        }

        if (ghost != null)
        {
            if (ghost.score >= 0)
            {
                ghost.score--;
            }
            ghost.DisableHighlight();
        }

        activeQuests.Remove(progress);
        UpdateQuestLog();

        if (reassignGhost && ghost != null)
        {
            ChairManager.Instance.AssignGhost(ghost.gameObject);
        }

        return true;
    }

    /// <summary>
    /// Completes the current ReturnToNPC objective for an active quest
    /// assigned by the specified quest giver.
    /// </summary>
    /// <param name="questGiver">The NPC the player has returned to.</param>
    /// <returns>True if a matching return-to-NPC objective was completed; otherwise, false.</returns>
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

        endDayUI.completedQuests++;

        Quest completedQuest = progress.quest;
        QuestGiver questGiver = progress.questGiver;

        Debug.Log("Completed quest: " + completedQuest.questName, this);
        GhostScript tempGhostScript = questGiver.transform.parent.GetComponent<GhostScript>();
        tempGhostScript.DisableHighlight();
        tempGhostScript.customerTimer += tempGhostScript.maxCustomerTimer / 2; 
        tempGhostScript.maxCustomerTimer = tempGhostScript.customerTimer;

        // test for adding points
        dayCycle.AddPoints(1);
        if (questGiver.transform.parent.GetComponent<GhostScript>().score < 5)
        {
            questGiver.transform.parent.GetComponent<GhostScript>().score++;
        }

        if (onAnyQuestCompleted != null)
                onAnyQuestCompleted.Invoke();

        QuestCompleted?.Invoke(completedQuest, questGiver);
        UpdateQuestLog();
        ChairManager.Instance.AssignGhost(questGiver.transform.parent.gameObject);
    }

    private bool MatchesCurrentObjective(
        Quest.QuestProgress progress,
        Quest.objectiveType type,
        MicrogameId gameID)
    {
        Quest.QuestObjective objective = progress.CurrentObjective;

        return objective != null &&
               objective.type == type &&
               objective.gameID == gameID;
    }

    /// <summary>
    /// Finds the first active, incomplete quest whose current objective
    /// is a microgame objective matching the specified microgame ID.
    /// </summary>
    /// <param name="gameID">The microgame ID to search for.</param>
    /// <returns>The matching quest progress, or null if no matching objective is found.</returns>
    public Quest.QuestProgress GetNextMicrogameProgress(MicrogameId gameID)
    {
        if (activeQuests == null)
            return null;

        foreach (Quest.QuestProgress progress in activeQuests)
        {
            if (progress == null || progress.IsCompleted)
                continue;

            if (MatchesCurrentObjective(
                progress,
                Quest.objectiveType.Microgame,
                gameID))
            {
                return progress;
            }
        }

        return null;
    }

    private void PrepareBookRequest(Quest.QuestProgress progress)
    {
        foreach (Quest.QuestObjective objective in progress.quest.objectives)
        {
            if (objective == null ||
                objective.type != Quest.objectiveType.Microgame ||
                objective.gameID != MicrogameId.BookRequestGame)
            {
                continue;
            }

            BookRequestGame bookGame =
                FindFirstObjectByType<BookRequestGame>(
                    FindObjectsInactive.Include);

            if (bookGame == null)
            {
                Debug.LogError("BookRequestGame not found.", this);
                return;
            }

            bookGame.PrepareRequest(progress);

            if (bookGame.requestDialogue != null &&
                bookGame.requestDialogue.DialogText != null &&
                bookGame.requestDialogue.DialogText.Length > 0)
            {
                bookGame.requestDialogue.DialogText[0] =
                    bookGame.GetRequestDescription(progress);
            }

            return;
        }
    }

    /// <summary>
    /// Removes all active quests, updates the quest log,
    /// and logs that the quests have been cleared.
    /// </summary>
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