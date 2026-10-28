using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the quest log UI by creating entries for active quests
/// and displaying their objectives with the appropriate text colors.
/// </summary>
public class QuestLogController : MonoBehaviour
{
    [Header("Quest Log")]
    [Tooltip("The UI GameObject used to display the quest log.")]
    public GameObject questLogUI;

    [Tooltip("The Transform that holds the instantiated quest entries.")]
    public Transform questList;

    [Tooltip("The prefab instantiated for each active quest in the quest log.")]
    public GameObject questEntryPrefab;

    [Header("Text Colors")]
    [Tooltip("The color used for the quest name and objectives that have not been completed or are not current.")]
    public Color textColor = Color.white;

    [Tooltip("The color used to highlight the current quest objective.")]
    public Color currentTextColor = Color.green;

    [Tooltip("The color used to display completed quest objectives.")]
    public Color completedTextColor = Color.gray;

    private void Start()
    {
        if (questLogUI != null)
            questLogUI.SetActive(true);

        UpdateQuestLog();
    }

    /// <summary>
    /// Refreshes the quest log by clearing the existing entries and creating
    /// a new entry for each active, incomplete quest.
    /// Logs a warning if required UI references are missing.
    /// </summary>
    public void UpdateQuestLog()
    {
        if (questList == null)
        {
            Debug.LogWarning("QuestLogController is missing its Quest List reference.", this);
            return;
        }

        ClearQuestEntries();

        QuestController controller = QuestController.Instance;
        if (controller == null || controller.activeQuests == null)
            return;

        if (questEntryPrefab == null)
        {
            Debug.LogWarning("QuestLogController is missing its Quest Entry Prefab.", this);
            return;
        }

        foreach (Quest.QuestProgress progress in controller.activeQuests)
        {
            if (progress == null || progress.IsCompleted || progress.quest == null)
                continue;

            CreateQuestEntry(progress);
        }
    }

    private void ClearQuestEntries()
    {
        for (int i = questList.childCount - 1; i >= 0; i--)
            Destroy(questList.GetChild(i).gameObject);
    }

    private void CreateQuestEntry(Quest.QuestProgress progress)
    {
        GameObject entry = Instantiate(questEntryPrefab, questList);
        TextMeshProUGUI text = entry.GetComponent<TextMeshProUGUI>();

        if (text == null)
            text = entry.GetComponentInChildren<TextMeshProUGUI>();

        if (text == null)
        {
            Debug.LogError("The Quest Entry Prefab needs a TextMeshProUGUI component.", entry);
            Destroy(entry);
            return;
        }

        text.text = BuildQuestText(progress);
    }
    private string BuildQuestText(Quest.QuestProgress progress)
    {
        Quest quest = progress.quest;
        StringBuilder builder = new StringBuilder();

        builder.Append("<b><color=#")
            .Append(ColorUtility.ToHtmlStringRGBA(textColor))
            .Append(">")
            .Append(quest.questName)
            .Append(":</color></b>\n");

        if (quest.objectives == null)
            return builder.ToString();

        for (int i = 0; i < quest.objectives.Count; i++)
        {
            Quest.QuestObjective objective = quest.objectives[i];

            if (objective == null)
                continue;

            // Hide objectives until they become the current objective.
            if (i > progress.currentObjectiveIndex)
                continue;

            if (i < progress.currentObjectiveIndex)
            {
                // Completed objective.
                builder.Append("<color=#")
                    .Append(ColorUtility.ToHtmlStringRGBA(completedTextColor))
                    .Append(">[Done] ")
                    .Append(objective.description)
                    .Append("</color>\n");
            }
            else
            {
                // Current objective.
                builder.Append("<b><color=#")
                    .Append(ColorUtility.ToHtmlStringRGBA(currentTextColor))
                    .Append(">→ ")
                    .Append(objective.description)
                    .Append("</color></b>\n");
            }
        }

        return builder.ToString();
    }
}