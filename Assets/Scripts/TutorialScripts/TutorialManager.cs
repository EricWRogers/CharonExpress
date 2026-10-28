
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    private class TutorialStep
    {
        public MicrogameId task;
        public DialogGraph dialogueAfterCompletion;

        public bool startEscortAfterDialogue;
    }

    [Header("Tutorial Steps")]
    [SerializeField] private TutorialStep[] steps;

    private QuestController questController;
    private GhostScript tutorialGhost;
    private Dialogue tutorialDialogue;

    private bool tutorialGhostConfigured;
    private int nextStepIndex;

    private void Start()
    {
        questController = QuestController.Instance;

        if (questController == null)
        {
            Debug.LogError("QuestController was not found.", this);
            enabled = false;
            return;
        }

        questController.ObjectiveCompleted += OnObjectiveCompleted;

        // The ghost might not have spawned yet.
        TrySetupTutorialGhost();
    }

    private void Update()
    {
        // Keep checking until the spawned ghost is configured.
        if (!tutorialGhostConfigured)
            TrySetupTutorialGhost();
    }

    private void TrySetupTutorialGhost()
    {
        ChairManager chairManager = ChairManager.Instance;

        if (chairManager == null || chairManager.chairs == null)
            return;

        foreach (GameObject chairObject in chairManager.chairs)
        {
            if (chairObject == null)
                continue;

            ChairScript chair =
                chairObject.GetComponent<ChairScript>();

            if (chair == null || chair.ghostObject == null)
                continue;

            GhostScript ghost =
                chair.ghostObject.GetComponent<GhostScript>();

            if (ghost == null)
                continue;

            SetupTutorialGhost(ghost);
            return;
        }
    }

    private void SetupTutorialGhost(GhostScript ghost)
    {
        if (ghost == null)
            return;

        // Find Dialogue on the spawned ghost or one of its children.
        Dialogue dialogue =
            ghost.GetComponentInChildren<Dialogue>(true);

        if (dialogue == null)
        {
            Debug.LogError(
                "The spawned tutorial ghost has no Dialogue component.",
                ghost.gameObject);

            // Avoid repeatedly checking the same ghost.
            tutorialGhostConfigured = true;
            return;
        }

        tutorialGhost = ghost;
        tutorialDialogue = dialogue;

        // Connect the dialogue to its own spawned ghost.
        tutorialDialogue.ghost = tutorialGhost;

        // Find the quest giver on the ghost prefab if one is present.
        if (tutorialDialogue.questGiver == null)
        {
            QuestGiver questGiver =
                tutorialGhost.GetComponentInChildren<QuestGiver>(true);

            if (questGiver != null)
                tutorialDialogue.questGiver = questGiver.gameObject;
        }

        tutorialGhostConfigured = true;
    }

    private void OnDestroy()
    {
        if (questController != null)
            questController.ObjectiveCompleted -= OnObjectiveCompleted;
    }

    private void OnObjectiveCompleted(
        Quest.QuestProgress progress,
        Quest.QuestObjective objective)
    {
        if (objective == null ||
            objective.type != Quest.objectiveType.Microgame)
        {
            return;
        }

        if (tutorialDialogue == null)
        {
            Debug.LogWarning(
                "The tutorial dialogue has not been initialized yet.",
                this);
            return;
        }

        if (steps == null || nextStepIndex >= steps.Length)
            return;

        TutorialStep step = steps[nextStepIndex];

        if (step == null)
        {
            Debug.LogError(
                $"Tutorial step {nextStepIndex} is not configured.",
                this);
            return;
        }

        if (objective.gameID != step.task)
        {
            Debug.LogWarning(
                $"Expected tutorial task {step.task}, " +
                $"but received {objective.gameID}.",
                this);
            return;
        }

        if (step.dialogueAfterCompletion == null)
        {
            Debug.LogError(
                $"No dialogue assigned after {step.task}.",
                this);
            return;
        }

        tutorialDialogue.SetDialogGraph(step.dialogueAfterCompletion);

        // Only start an escort for steps configured to do so.
        tutorialDialogue.escortMission = step.startEscortAfterDialogue;

        nextStepIndex++;
    }

    public bool ShouldPlayReturnDialogue(QuestGiver questGiver)
    {
        if (!tutorialGhostConfigured ||
            tutorialGhost == null ||
            tutorialDialogue == null ||
            questGiver == null)
        {
            return false;
        }

        // Only apply this behavior to the tutorial ghost.
        if (questGiver.GetComponentInParent<GhostScript>() != tutorialGhost)
            return false;

        QuestController controller = QuestController.Instance;

        if (controller == null || controller.activeQuests == null)
            return false;

        foreach (Quest.QuestProgress progress in controller.activeQuests)
        {
            if (progress == null ||
                progress.IsCompleted ||
                progress.questGiver != questGiver)
            {
                continue;
            }

            Quest.QuestObjective objective = progress.CurrentObjective;

            return objective != null &&
                objective.type == Quest.objectiveType.ReturnToNPC;
        }

        return false;
    }
}
