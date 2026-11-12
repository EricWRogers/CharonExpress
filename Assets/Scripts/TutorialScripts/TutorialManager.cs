
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    private class TutorialStep
    {
        public MicrogameId task;
        public DialogGraph dialogueWhileInProgress;
        public DialogGraph dialogueAfterCompletion;
        public DialogGraph dialogueAfterFailure;
    }

    [Header("Tutorial Steps")]
    [SerializeField] private TutorialStep[] steps;

    private QuestController questController;
    private GhostScript tutorialGhost;
    private Dialogue tutorialDialogue;
    private int nextStepIndex;

    [Header("Repeat Quest Dialogue")]
        [SerializeField] private DialogGraph[] repeatQuestDialogues;
        [SerializeField] private int[] dialogueThresholds;
        private int completedTutorialQuests;

    public bool isTutorialActive = true;


    private IEnumerator Start()
    {
        questController = QuestController.Instance;

        questController.QuestCompleted += OnTutorialQuestCompleted;
        if (questController != null)
        {
            questController.ObjectiveCompleted += OnObjectiveCompleted;
            questController.QuestFailed += OnQuestFailed;
        }
        else
            Debug.LogError("QuestController was not found.", this);

        // Wait for ChairManager to initialize.
        yield return null;

        ChairManager chairManager = ChairManager.Instance;

        if (chairManager == null || chairManager.chairs == null ||
            chairManager.activeGhosts >= chairManager.maxGhostCount)
        {
            Debug.LogError("Unable to spawn tutorial ghost.", this);
            yield break;
        }

        // Find an available chair.
        ChairScript chair = null;

        foreach (GameObject obj in chairManager.chairs)
        {
            if (obj == null) continue;

            ChairScript candidate = obj.GetComponent<ChairScript>();

            if (candidate != null && !candidate.reserved &&
                !candidate.ghostActive && candidate.ghostObject == null &&
                candidate.ghostPrefab != null)
            {
                chair = candidate;
                break;
            }
        }

        if (chair == null)
        {
            Debug.LogError("No available chair for tutorial ghost.", this);
            yield break;
        }

        // Spawn and configure the ghost.
        GameObject ghostObject = Instantiate(
            chair.ghostPrefab,
            chair.transform.position + Vector3.up,
            Quaternion.identity);

        tutorialGhost = ghostObject.GetComponent<GhostScript>();
        tutorialDialogue = ghostObject.GetComponent<Dialogue>();

        if (tutorialGhost == null || tutorialDialogue == null)
        {
            Debug.LogError("Ghost prefab requires GhostScript and Dialogue.", ghostObject);
            Destroy(ghostObject);
            tutorialGhost = null;
            tutorialDialogue = null;
            yield break;
        }

        chair.ghostObject = ghostObject;
        chair.ghostActive = true;
        chairManager.activeGhosts++;

        tutorialGhost.chairScript = chair;
        tutorialGhost.customerTimer = Random.Range(45, 60);

        chairManager.AssignGhostAsspects(tutorialGhost, ghostObject);
        tutorialGhost.ghostScriptableObject.needExtraTask = false;

        // Configure the quest giver and interaction.
        QuestGiver giver = ghostObject.GetComponentInChildren<QuestGiver>(true);
        DialogInteraction interaction = giver != null
            ? giver.text
            : null;

        if (giver != null)
            tutorialDialogue.questGiver = giver.gameObject;

        if (interaction == null)
            interaction = ghostObject.GetComponentInChildren<DialogInteraction>(true);

        if (interaction == null)
        {
            Debug.LogError("Tutorial ghost has no DialogInteraction.", ghostObject);
            yield break;
        }

        interaction.text = tutorialDialogue;

        // Let the ghost's components initialize before starting dialogue.
        yield return null;

        while (TextBoxManager.Instance != null &&
               TextBoxManager.Instance.NoTalk &&
               !tutorialDialogue.textActive)
        {
            yield return null;
        }

        if (TextBoxManager.Instance != null &&
            !tutorialDialogue.textActive)
        {
            interaction.StartDiologue();
        }
    }

    private void OnObjectiveCompleted(
        Quest.QuestProgress progress,
        Quest.QuestObjective objective)
    {
        if (progress == null || objective == null ||
            tutorialDialogue == null ||
            progress.questGiver == null ||
            progress.questGiver.GetComponentInParent<GhostScript>() != tutorialGhost ||
            steps == null || steps.Length == 0)
            return;

        tutorialGhost.ghostScriptableObject.needExtraTask = false;

        // Quest accepted: keep the waiting dialogue.
        if (objective.type == Quest.objectiveType.FirstTalk)
        {
            TutorialStep waitingStep =
                steps[Mathf.Min(nextStepIndex, steps.Length - 1)];

            if (waitingStep != null && waitingStep.dialogueWhileInProgress != null)
                tutorialDialogue.SetDialogGraph(waitingStep.dialogueWhileInProgress);

            return;
        }

        if (objective.type != Quest.objectiveType.Microgame)
            return;

        // Complete the original tutorial steps normally.
        if (nextStepIndex < steps.Length)
        {
            TutorialStep step = steps[nextStepIndex];

            if (step == null || objective.gameID != step.task)
                return;

            if (step.dialogueAfterCompletion != null)
                tutorialDialogue.SetDialogGraph(step.dialogueAfterCompletion);

            nextStepIndex++;
            return;
        }

        // Repeated quests: use the last step's success dialogue as the default.
        TutorialStep repeatStep = steps[steps.Length - 1];

        if (repeatStep == null || objective.gameID != repeatStep.task)
            return;

        DialogGraph successDialogue = GetRepeatSuccessDialogue(
            repeatStep.dialogueAfterCompletion);

        if (successDialogue != null)
            tutorialDialogue.SetDialogGraph(successDialogue);
    }
    public bool ShouldPlayReturnDialogue(QuestGiver questGiver)
    {
        if (tutorialGhost == null || questGiver == null ||
            questGiver.GetComponentInParent<GhostScript>() != tutorialGhost)
            return false;

        QuestController controller = QuestController.Instance;

        if (controller == null || controller.activeQuests == null)
            return false;

        foreach (Quest.QuestProgress progress in controller.activeQuests)
        {
            if (progress == null || progress.IsCompleted ||
                progress.questGiver != questGiver)
                continue;

            Quest.QuestObjective objective = progress.CurrentObjective;

            return objective != null &&
                objective.type == Quest.objectiveType.ReturnToNPC;
        }

        return false;
    }

    private void OnQuestFailed(
        Quest.QuestProgress progress,
        Quest.QuestObjective objective)
    {
        if (progress == null || objective == null ||
            tutorialDialogue == null ||
            progress.questGiver == null ||
            progress.questGiver.GetComponentInParent<GhostScript>() != tutorialGhost ||
            objective.type != Quest.objectiveType.Microgame ||
            steps == null || steps.Length == 0)
            return;

        int index = Mathf.Min(nextStepIndex, steps.Length - 1);
        TutorialStep step = steps[index];

        if (step == null || objective.gameID != step.task)
            return;

        // Failures always use the default dialogue.
        if (step.dialogueAfterFailure != null)
            tutorialDialogue.SetDialogGraph(step.dialogueAfterFailure);

        // Advance only while processing the original tutorial steps.
        if (nextStepIndex < steps.Length)
            nextStepIndex++;
    }
    private void OnTutorialQuestCompleted(Quest quest, QuestGiver giver)
    {
        if (giver == null ||
            giver.GetComponentInParent<GhostScript>() != tutorialGhost)
            return;

        completedTutorialQuests++;
    }

    private DialogGraph GetRepeatSuccessDialogue(DialogGraph defaultDialogue)
    {
        DialogGraph result = defaultDialogue;

        int count = Mathf.Min(
            dialogueThresholds.Length,
            repeatQuestDialogues.Length);

        for (int i = 0; i < count; i++)
        {
            if (completedTutorialQuests >= dialogueThresholds[i] &&
                repeatQuestDialogues[i] != null)
            {
                result = repeatQuestDialogues[i];
            }
        }

        return result;
    }
}
