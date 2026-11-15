
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages quest destination arrows by tracking active quest objectives.
/// Creates, updates, and removes arrows as quest destinations change.
/// </summary>
public class QuestArrowManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Prefab used to create quest destination arrows.")]
    [SerializeField] private QuestArrow m_arrowPrefab;

    [Tooltip("Parent transform used to organize quest arrows in the UI.")]
    [SerializeField] private RectTransform m_arrowParent;

    [Tooltip("Camera used to calculate quest arrow positioning.")]
    [SerializeField] private Camera m_mainCamera;


    private QuestController m_questController;
    private bool m_isSubscribed;

    private readonly Dictionary<Transform, QuestArrow> m_activeArrows = new();


    private void Start()
    {
        if (m_arrowPrefab == null ||
            m_arrowParent == null ||
            m_mainCamera == null)
        {
            Debug.LogError("QuestArrowManager is missing references.", this);
            enabled = false;
            return;
        }

        m_questController = QuestController.Instance;

        if (m_questController == null)
        {
            Debug.LogError("QuestController was not found.", this);
            enabled = false;
            return;
        }

        SubscribeToQuestChanges();
        RefreshArrows();
    }


    private void OnEnable()
    {
        SubscribeToQuestChanges();
        RefreshArrows();
    }


    private void OnDisable()
    {
        UnsubscribeFromQuestChanges();
        ClearArrows();
    }


    private void SubscribeToQuestChanges()
    {
        if (m_questController == null || m_isSubscribed)
            return;

        m_questController.QuestStateChanged += RefreshArrows;
        m_isSubscribed = true;
    }


    private void UnsubscribeFromQuestChanges()
    {
        if (m_questController == null || !m_isSubscribed)
            return;

        m_questController.QuestStateChanged -= RefreshArrows;
        m_isSubscribed = false;
    }


    private void ClearArrows()
    {
        foreach (QuestArrow arrow in m_activeArrows.Values)
        {
            if (arrow != null)
                Destroy(arrow.gameObject);
        }

        m_activeArrows.Clear();
    }


    private void RefreshArrows()
    {
        if (m_questController == null)
            return;

        Dictionary<MicrogameId, Transform> stations = new();

        foreach (MicroGameStart station in FindObjectsByType<MicroGameStart>(
                     FindObjectsSortMode.None))
        {
            if (station != null && !stations.ContainsKey(station.gameID))
                stations.Add(station.gameID, station.transform);
        }

        HashSet<Transform> destinations = new();

        foreach (Quest.QuestProgress progress in m_questController.activeQuests)
        {
            if (progress == null)
                continue;

            Quest.QuestObjective objective = progress.CurrentObjective;

            if (objective == null)
                continue;

            Transform destination = null;

            switch (objective.type)
            {
                case Quest.objectiveType.Microgame:
                    stations.TryGetValue(objective.gameID, out destination);
                    break;

                case Quest.objectiveType.ReturnToNPC:
                    if (progress.questGiver != null)
                        destination = progress.questGiver.transform;
                    break;
            }

            if (destination != null)
                destinations.Add(destination);
        }

        List<Transform> toRemove = new();

        foreach (var pair in m_activeArrows)
        {
            if (!destinations.Contains(pair.Key))
            {
                if (pair.Value != null)
                    Destroy(pair.Value.gameObject);

                toRemove.Add(pair.Key);
            }
        }

        foreach (Transform destination in toRemove)
            m_activeArrows.Remove(destination);

        foreach (Transform destination in destinations)
        {
            if (m_activeArrows.TryGetValue(destination, out QuestArrow arrow) &&
                arrow != null)
            {
                continue;
            }

            arrow = Instantiate(m_arrowPrefab, m_arrowParent);
            arrow.Initialize(m_mainCamera, destination);

            m_activeArrows[destination] = arrow;
        }
    }
}
