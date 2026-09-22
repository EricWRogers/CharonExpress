using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestLogController : MonoBehaviour
{
    [Header("Quest Log")]
    public GameObject questLogUI;
    public Transform questList;
    public GameObject questEntryPrefab;

    [Header("Quest Log Key")]
    public Key keyToOpen = Key.Tab;

    [Header("Text Colors")]
    public Color textColor = Color.white;

    public Color currentTextColor = Color.green;
    public Color completedTextColor = Color.gray;

    private void Start()
    {
        //questLogUI.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current[keyToOpen].wasPressedThisFrame)
        {
            ToggleQuestLog();
        }
    }

    public void ToggleQuestLog()
    {
        if (questLogUI.activeSelf)
        {
            questLogUI.SetActive(false);
        }
        else
        {
            UpdateQuestLog();
            questLogUI.SetActive(true);
        }
    }

    public void UpdateQuestLog()
    {
        foreach (Transform child in questList)
        {
            Destroy(child.gameObject);
        }

        foreach (Quest.QuestProgress progress in QuestController.Instance.activeQuests)
        {
            if (progress.IsCompleted)
                continue;

            GameObject entry = Instantiate(
                questEntryPrefab,
                questList
            );

            TextMeshProUGUI text =
                entry.GetComponent<TextMeshProUGUI>();

            if (text == null)
                continue;

            Quest quest = progress.quest;

            string normalColor = ColorUtility.ToHtmlStringRGBA(textColor);
            string completedColor = ColorUtility.ToHtmlStringRGBA(completedTextColor);

            string questText =
                "<b><color=#" + normalColor + ">" +
                quest.questName + ":" +
                "</color></b>\n";
            for (int i = 0; i < quest.objectives.Count; i++)
            {
                Quest.QuestObjective objective = quest.objectives[i];

                if (i < progress.currentObjectiveIndex)
                {
                    // Completed objective.
                    questText +=
                        "<color=#" +
                        ColorUtility.ToHtmlStringRGBA(completedTextColor) +
                        ">[Done] " +
                        objective.description +
                        "</color>\n";
                }
                else if (i == progress.currentObjectiveIndex)
                {
                    // Current objective.
                    questText +=
                        "<b><color=#" +
                        ColorUtility.ToHtmlStringRGBA(currentTextColor) +
                        ">→ " +
                        objective.description +
                        "</color></b>\n";
                }
                else
                {
                    // Future objective.
                    questText +=
                        "<color=#" +
                        ColorUtility.ToHtmlStringRGBA(textColor) +
                        ">" +
                        objective.description +
                        "</color>\n";
                }
            }
            text.text = questText;
        }
    }
}