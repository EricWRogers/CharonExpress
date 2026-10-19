using UnityEngine;
using System;
using NUnit.Framework;

public class DayCycle : MonoBehaviour
{
    public enum TravelState
    {
        Traveling,
        AtStation
    }

    public float SecondsBetweenStations = 180f;
    public int firstDay = 1;
    public bool pauseGameAtStation = true;

    private int currentDay;
    private float secondsRemaining;
    private int totalPoints;
    private int pointsEarnedToday;

    private TravelState state;

    private float PreviousTimeScale;
    private bool IsPaused;

    // Read-only information that other scripts can access.
    public int CurrentDay => currentDay;
    public float SecondsRemaining => secondsRemaining;
    public int TotalPoints => totalPoints;
    public int PointsEarnedToday => pointsEarnedToday;
    public bool IsAtStation => state == TravelState.AtStation;

    public event Action StationReached;
    public event Action TravelStarted;

    private void Awake()
    {
        if (SecondsBetweenStations <= 1f)
        {
            Debug.LogWarning("SecondsBetweenStations must be greater than 1 second. Setting to 1 second.", this);
            SecondsBetweenStations = 1f;
        }

        currentDay = Mathf.Max(1, firstDay);
        secondsRemaining = SecondsBetweenStations;
        state = TravelState.Traveling;
    }

    private void Update()
    {
        if (state != TravelState.Traveling)
            return;
        
        secondsRemaining = Mathf.Max(0f, secondsRemaining - Time.deltaTime);

        if (secondsRemaining <= 0f)
        {
            ReachStation();
        }
    }
    private void ReachStation()
    {
        if (state != TravelState.Traveling)
            return;

        state = TravelState.AtStation;
        secondsRemaining = 0f;

        if (pauseGameAtStation && Time.timeScale > 0f)
        {
            PreviousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            IsPaused = true;
        }

        StationReached?.Invoke();
    }

    public void AddPoints(int amount)
    {
        if (amount <= 0)
        {
            if (amount < 0)
            {
                Debug.LogWarning("AddPoints called with a negative amount. Ignoring.", this);
            }
            return;
        }
        totalPoints += amount;
        pointsEarnedToday += amount;
    }

    public void DepartStation()
    {
        if (state != TravelState.AtStation)
            return;

        RestoreTimeScale();

        currentDay++;
        pointsEarnedToday = 0;
        secondsRemaining = SecondsBetweenStations;
        state = TravelState.Traveling;

        TravelStarted?.Invoke();
    }
    private void RestoreTimeScale()
    {
        if (!IsPaused)
            return;
        Time.timeScale = PreviousTimeScale;
        IsPaused = false;
    }
}
