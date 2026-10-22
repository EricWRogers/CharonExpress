
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndDayUI : MonoBehaviour
{
    [Header("Systems")]
    public DayCycle daySystem;
    public StationArrivalController stationArrivalController;

    [Header("Travel HUD")]
    public TMP_Text dayLabel;
    public TMP_Text countdownLabel;

    [Header("Station Summary")]
    public GameObject stationSummaryPanel;
    public TMP_Text stationSummaryLabel;
    public Button continueButton;
    public GameManager gameManager;
    public int failedQuests = 0;
    public int completedQuests = 0;

    private int lastDisplayedSecond = -1;

    private void Awake()
    {
        if (daySystem == null ||
            stationArrivalController == null)
        {
            Debug.LogError(
                "DaySystemUI is missing required references.",
                this
            );

            enabled = false;
            return;
        }

        if (stationSummaryPanel != null)
            stationSummaryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (daySystem != null)
            daySystem.TravelStarted += ShowTravelHUD;

        if (stationArrivalController != null)
        {
            stationArrivalController.PresentationReady +=
                ShowStationSummary;

            if (stationArrivalController.IsReadyForContinue)
                ShowStationSummary();
        }

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinuePressed);
    }

    private void Start()
    {
        if (daySystem == null)
            return;

        UpdateDayLabel();
        UpdateCountdown(true);
    }

    private void Update()
    {
        if (daySystem != null)
            UpdateCountdown();
    }

    private void OnDisable()
    {
        if (daySystem != null)
            daySystem.TravelStarted -= ShowTravelHUD;

        if (stationArrivalController != null)
        {
            stationArrivalController.PresentationReady -=
                ShowStationSummary;
        }

        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinuePressed);
    }

    private void UpdateDayLabel()
    {
        if (dayLabel != null)
            dayLabel.text = $"Day {daySystem.currentDay}";
    }

    private void UpdateCountdown(bool forceRefresh = false)
    {
        int totalSeconds = Mathf.CeilToInt(
            daySystem.secondsRemaining
        );

        if (!forceRefresh && totalSeconds == lastDisplayedSecond)
            return;

        lastDisplayedSecond = totalSeconds;

        if (countdownLabel == null)
            return;

        if (daySystem.state == DayCycle.TravelState.AtStation)
        {
            countdownLabel.text = "Station reached";
        }
        else
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            countdownLabel.text =
                $"Next station in {minutes:00}:{seconds:00}";
        }
    }

    private void ShowStationSummary()
    {
        if (stationSummaryPanel != null)
            stationSummaryPanel.SetActive(true);

        if (stationSummaryLabel != null)
        {
            /*
            stationSummaryLabel.text =
                $"Day {daySystem.currentDay} Complete!\n\n" +
                $"Points earned today: {daySystem.pointsEarnedToday}\n" +
                $"Total points: {daySystem.totalPoints}";
                */
            stationSummaryLabel.text =
                $"End of Demo!\n\n" +
                $"Completed Quests {completedQuests}\n" +
                $"Failed Quests {failedQuests}\n" +
                $"Grade: {gameManager.letterGrade}\n";
        }

        UpdateCountdown(true);
    }

    private void ShowTravelHUD()
    {
        if (stationSummaryPanel != null)
            stationSummaryPanel.SetActive(false);

        UpdateDayLabel();
        UpdateCountdown(true);
    }

    private void OnContinuePressed()
    {
        if (stationArrivalController == null)
            return;

        if (!stationArrivalController.IsReadyForContinue)
            return;

        if (stationSummaryPanel != null)
            stationSummaryPanel.SetActive(false);

        stationArrivalController.ContinueJourney();
    }
}
