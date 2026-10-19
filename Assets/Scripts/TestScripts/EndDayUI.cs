
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DaySystemUI : MonoBehaviour
{
    [Header("Day System")]
    public DayCycle daySystem;

    [Header("Travel HUD")]
    public TMP_Text dayLabel;
    public TMP_Text countdownLabel;

    [Header("Station Summary")]
    public GameObject stationSummaryPanel;
    public TMP_Text stationSummaryLabel;
    public Button continueButton;

    private int lastDisplayedSecond = -1;

    private void Awake()
    {
        if (daySystem == null)
        {
            Debug.LogError(
                "DaySystemUI needs a DaySystem reference.",
                this
            );

            enabled = false;
            return;
        }

        if (stationSummaryPanel != null)
        {
            stationSummaryPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (daySystem != null)
        {
            daySystem.StationReached += ShowStationSummary;
            daySystem.TravelStarted += ShowTravelHUD;
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinuePressed);
        }
    }

    private void Start()
    {
        if (daySystem == null)
            return;

        UpdateDayLabel();
        UpdateCountdown(true);

        if (stationSummaryPanel != null)
        {
            stationSummaryPanel.SetActive(daySystem.IsAtStation);
        }

        if (daySystem.IsAtStation)
        {
            ShowStationSummary();
        }
    }

    private void Update()
    {
        if (daySystem == null)
            return;

        UpdateCountdown();
    }

    private void OnDisable()
    {
        if (daySystem != null)
        {
            daySystem.StationReached -= ShowStationSummary;
            daySystem.TravelStarted -= ShowTravelHUD;
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinuePressed);
        }
    }

    private void UpdateDayLabel()
    {
        if (dayLabel != null)
        {
            dayLabel.text = $"Day {daySystem.CurrentDay}";
        }
    }

    private void UpdateCountdown(bool forceRefresh = false)
    {
        int totalSeconds = Mathf.CeilToInt(
            daySystem.SecondsRemaining
        );

        if (!forceRefresh && totalSeconds == lastDisplayedSecond)
            return;

        lastDisplayedSecond = totalSeconds;

        if (countdownLabel == null)
            return;

        if (daySystem.IsAtStation)
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
        {
            stationSummaryPanel.SetActive(true);
        }

        if (stationSummaryLabel != null)
        {
            stationSummaryLabel.text =
                $"Day {daySystem.CurrentDay} Complete!\n\n" +
                $"Points earned today: {daySystem.PointsEarnedToday}\n" +
                $"Total points: {daySystem.TotalPoints}";
        }

        UpdateCountdown(true);
    }

    private void ShowTravelHUD()
    {
        if (stationSummaryPanel != null)
        {
            stationSummaryPanel.SetActive(false);
        }

        UpdateDayLabel();
        UpdateCountdown(true);
    }

    private void OnContinuePressed()
    {
        daySystem.DepartStation();
    }
}
