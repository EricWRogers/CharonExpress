
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

    [Tooltip("Vertical screen offset for arrows pointing to Escort chairs.")]
    [SerializeField] private float m_chairArrowYOffset = 200f;
    [SerializeField] private float m_chairArrowXOffset = 5f;

    private QuestController m_questController;
    private bool m_isInitialized;

    private readonly Dictionary<MicrogameId, Transform> m_stations = new();
    private readonly Dictionary<Transform, QuestArrow> m_activeArrows = new();

    private void Start()
    {
        if (m_arrowPrefab == null || m_arrowParent == null || m_mainCamera == null)
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

        CacheMicrogameStations();

        m_isInitialized = true;
        m_questController.QuestStateChanged += RefreshArrows;
        RefreshArrows();
    }

    private void CacheMicrogameStations()
    {
        m_stations.Clear();

        foreach (MicroGameStart station in FindObjectsByType<MicroGameStart>(
                     FindObjectsSortMode.None))
        {
            if (station != null && !m_stations.ContainsKey(station.gameID))
                m_stations.Add(station.gameID, station.transform);
        }
    }

    private Transform GetEscortSeat()
    {
        ChairManager chairManager = ChairManager.Instance;

        if (chairManager == null || chairManager.escortChair < 0)
            return null;

        int index = 0;

        foreach (var chair in chairManager.chairs)
        {
            if (index == chairManager.escortChair)
                return chair != null ? chair.transform : null;

            index++;
        }

        return null;
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

        HashSet<Transform> destinations = new();
        Transform escortSeat = GetEscortSeat();

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
                    if (objective.gameID == MicrogameId.Escort)
                        destination = escortSeat;
                    else
                        m_stations.TryGetValue(objective.gameID, out destination);
                    break;

                case Quest.objectiveType.ReturnToNPC:
                    if (progress.questGiver != null)
                        destination = progress.questGiver.transform;
                    break;
            }

            if (destination != null)
                destinations.Add(destination);
        }

        List<Transform> destinationsToRemove = null;

        foreach (KeyValuePair<Transform, QuestArrow> pair in m_activeArrows)
        {
            if (destinations.Contains(pair.Key))
                continue;

            if (pair.Value != null)
                Destroy(pair.Value.gameObject);

            if (destinationsToRemove == null)
                destinationsToRemove = new List<Transform>();

            destinationsToRemove.Add(pair.Key);
        }

        if (destinationsToRemove != null)
        {
            foreach (Transform destination in destinationsToRemove)
                m_activeArrows.Remove(destination);
        }

        foreach (Transform destination in destinations)
        {
            if (m_activeArrows.TryGetValue(destination, out QuestArrow arrow) &&
                arrow != null)
                continue;

            arrow = Instantiate(m_arrowPrefab, m_arrowParent);

            if (destination == escortSeat)
                arrow.Initialize(m_mainCamera, destination, m_chairArrowYOffset, m_chairArrowXOffset);
            else
                arrow.Initialize(m_mainCamera, destination);

            m_activeArrows[destination] = arrow;
        }
    }
}
