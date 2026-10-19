using System.Text;
using TMPro;
using UnityEngine;

public class QuestLogController : MonoBehaviour
{
    [Header("Quest Log")]
    public GameObject questLogUI;
    public Transform questList;
    public GameObject questEntryPrefab;

    [Header("Text Colors")]
    public Color textColor = Color.white;
    public Color currentTextColor = Color.green;
    public Color completedTextColor = Color.gray;

    private void Start()
    {
        if (questLogUI != null)
            questLogUI.SetActive(true);

        UpdateQuestLog();
    }

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

            if (i < progress.currentObjectiveIndex)
            {
                builder.Append("<color=#")
                    .Append(ColorUtility.ToHtmlStringRGBA(completedTextColor))
                    .Append(">[Done] ")
                    .Append(objective.description)
                    .Append("</color>\n");
            }
            else if (i == progress.currentObjectiveIndex)
            {
                builder.Append("<b><color=#")
                    .Append(ColorUtility.ToHtmlStringRGBA(currentTextColor))
                    .Append(">→ ")
                    .Append(objective.description)
                    .Append("</color></b>\n");
            }
            else
            {
                builder.Append("<color=#")
                    .Append(ColorUtility.ToHtmlStringRGBA(textColor))
                    .Append(">")
                    .Append(objective.description)
                    .Append("</color>\n");
            }
        }

        return builder.ToString();
    }
}
