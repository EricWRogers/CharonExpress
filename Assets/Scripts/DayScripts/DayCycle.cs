
using System;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public enum TravelState
    {
        Traveling,
        AtStation
    }

    [Header("Schedule")]
    [Min(1f)]
    public float SecondsBetweenStations = 180f;

    [Min(1)]
    public int firstDay = 1;

    public bool pauseGameAtStation = true;

    [Header("Current State")]
    [HideInInspector] public int currentDay;
    [HideInInspector] public float secondsRemaining;
    [HideInInspector] public int totalPoints;
    [HideInInspector] public int pointsEarnedToday;
    [HideInInspector] public TravelState state;

    private float previousTimeScale = 1f;
    private bool isPaused;

    public event Action StationReached;
    public event Action TravelStarted;
    public GameObject catchUI;
    public GameObject bookUI;
    public GameObject typingUI;

    private void Awake()
    {
        if (SecondsBetweenStations < 1f)
        {
            Debug.LogWarning(
                "Station duration must be at least one second.",
                this
            );

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

        if (!GhostScript.gamePaused) secondsRemaining = Mathf.Max(
            0f,
            secondsRemaining - Time.deltaTime
        );

        if (secondsRemaining <= 0f)
            ReachStation();
    }

    private void ReachStation()
    {
        if (state != TravelState.Traveling)
            return;

        state = TravelState.AtStation;
        secondsRemaining = 0f;

        if (pauseGameAtStation && Time.timeScale > 0f)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            isPaused = true;
        }

        catchUI.SetActive(false);
        bookUI.SetActive(false);
        typingUI.SetActive(false);
        GameManager.Instance.SetGrade();
        StationReached?.Invoke();
    }

    public void AddPoints(int amount)
    {
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
        if (!isPaused)
            return;

        Time.timeScale = previousTimeScale;
        isPaused = false;
    }

    private void OnDisable()
    {
        RestoreTimeScale();
    }
}
