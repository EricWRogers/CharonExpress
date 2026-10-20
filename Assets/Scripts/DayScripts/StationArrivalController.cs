using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class StationArrivalController : MonoBehaviour
{
    [Header("Systems")]
    public DayCycle daycycle;
    public CinemachineBrain cameraBrain;

    [Header("Cameras")]
    public CinemachineCamera playerCamera;
    public CinemachineCamera transitionCamera;
    public CinemachineCamera stationCamera;

    [Header("Station Movement Points")]
    public Transform stationRoot;
    public Transform offscreenPoint;
    public Transform dockPoint;
    public Transform departurePoint;

    [Header("Movement Settings")]
    [Min(0.01f)]
    public float stationMoveDuration = 1.5f;

    [Range(1f, 5f)]
    public float stationEaseOutStrength = 2f;

    [Min(0f)]
    public float transitionCameraHoldDuration = 0.5f;

    [Tooltip("If true, show the transition camera before moving the station on arrival. On departure, move the station before returning to the player camera.")]
    public bool transitionBeforeStationMovement = true;

    [Header("Camera Priorities")]
    public int playerPriority = 20;
    public int transitionActivePriority = 25;
    public int stationActivePriority = 30;
    public int idlePriority = 0;

    public event Action PresentationReady;

    private Coroutine activeSequence;
    private bool readyForContinue;

    public bool IsReadyForContinue => readyForContinue;

    private void Awake()
    {
        if (daycycle == null ||
            cameraBrain == null ||
            playerCamera == null ||
            transitionCamera == null ||
            stationCamera == null ||
            stationRoot == null ||
            offscreenPoint == null ||
            dockPoint == null ||
            departurePoint == null)
        {
            Debug.LogError(
                "StationArrivalController is missing references.",
                this
            );

            enabled = false;
            return;
        }

        // Allow camera blends and station movement while gameplay is paused.
        cameraBrain.IgnoreTimeScale = true;
    }

    private void OnEnable()
    {
        if (daycycle != null)
            daycycle.StationReached += HandleStationReached;
    }

    private void Start()
    {
        SetCameraPriority(playerCamera, playerPriority);
        SetCameraPriority(transitionCamera, idlePriority);
        SetCameraPriority(stationCamera, idlePriority);

        stationRoot.position = offscreenPoint.position;
    }

    private void OnDisable()
    {
        if (daycycle != null)
            daycycle.StationReached -= HandleStationReached;

        if (activeSequence != null)
        {
            StopCoroutine(activeSequence);
            activeSequence = null;
        }

        readyForContinue = false;

        if (playerCamera != null)
            SetCameraPriority(playerCamera, playerPriority);

        if (transitionCamera != null)
            SetCameraPriority(transitionCamera, idlePriority);

        if (stationCamera != null)
            SetCameraPriority(stationCamera, idlePriority);
    }

    private void HandleStationReached()
    {
        if (activeSequence != null || readyForContinue)
            return;

        if (QuestController.Instance != null)
        {
            QuestController.Instance.ClearAllQuests();
        }
        else
        {
            Debug.LogWarning(
                "QuestController was not found.",
                this
            );
        }

        activeSequence = StartCoroutine(ArrivalSequence());
    }

    private IEnumerator ArrivalSequence()
    {
        if (transitionBeforeStationMovement)
        {
            // Move to the transition camera first.
            yield return ShowTransitionCamera();

            // Move the station while the transition camera is active.
            yield return MoveStation(
                offscreenPoint.position,
                dockPoint.position
            );
        }
        else
        {
            // Move the station before switching to the transition camera.
            yield return MoveStation(
                offscreenPoint.position,
                dockPoint.position
            );

            yield return ShowTransitionCamera();
        }

        // Switch to the station camera and wait for the blend to finish.
        SetCameraPriority(stationCamera, stationActivePriority);
        yield return WaitForCamera(stationCamera);

        activeSequence = null;
        readyForContinue = true;

        PresentationReady?.Invoke();
    }

    public void ContinueJourney()
    {
        if (!readyForContinue || activeSequence != null)
            return;

        readyForContinue = false;
        activeSequence = StartCoroutine(DepartureSequence());
    }

    private IEnumerator DepartureSequence()
    {
        // Switch from the station camera to the transition camera.
        SetCameraPriority(stationCamera, idlePriority);
        yield return ShowTransitionCamera();

        if (transitionBeforeStationMovement)
        {
            // Move the station before returning to the player.
            yield return MoveStation(
                dockPoint.position,
                departurePoint.position
            );

            SetCameraPriority(transitionCamera, idlePriority);
            yield return WaitForCamera(playerCamera);
        }
        else
        {
            // Return to the player before moving the station.
            SetCameraPriority(transitionCamera, idlePriority);
            yield return WaitForCamera(playerCamera);

            yield return MoveStation(
                dockPoint.position,
                departurePoint.position
            );
        }

        daycycle.DepartStation();
        activeSequence = null;
    }

    private IEnumerator ShowTransitionCamera()
    {
        SetCameraPriority(
            transitionCamera,
            transitionActivePriority
        );

        yield return WaitForCamera(transitionCamera);

        if (transitionCameraHoldDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                transitionCameraHoldDuration
            );
        }
    }

    private IEnumerator WaitForCamera(CinemachineCamera targetCamera)
    {
        yield return null;

        while (cameraBrain != null)
        {
            UnityEngine.Object activeCamera =
                cameraBrain.ActiveVirtualCamera as UnityEngine.Object;

            if (activeCamera == targetCamera && !cameraBrain.IsBlending)
                break;

            yield return null;
        }
    }

    private IEnumerator MoveStation(Vector3 start, Vector3 end)
    {
        float duration = Mathf.Max(0.01f, stationMoveDuration);
        float elapsed = 0f;

        stationRoot.position = start;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float easedProgress = 1f - Mathf.Pow(
                1f - progress,
                stationEaseOutStrength
            );
            stationRoot.position = Vector3.Lerp(
                start,
                end,
                easedProgress
            );

            yield return null;
        }

        stationRoot.position = end;
    }

    private static void SetCameraPriority(
        CinemachineCamera camera,
        int priorityValue)
    {
        var priority = camera.Priority;
        priority.Enabled = true;
        priority.Value = priorityValue;
        camera.Priority = priority;
    }
}