
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class QuestArrow : MonoBehaviour
{
    [Header("Position Settings")]
    [Tooltip("Distance from the camera view edges, in pixels.")]
    [SerializeField] private float m_edgePadding = 50f;

    [Tooltip("Additional distance above the target, in screen pixels.")]
    [SerializeField] private float m_targetScreenOffset = 90f;


    [Header("Camera Entry Settings")]
    [Tooltip("How far inside the camera view the target must be before the arrow moves above it.")]
    [Range(0f, 0.45f)]
    [SerializeField] private float m_cameraEntryThreshold = 0.12f;

    [Tooltip("How close to the screen edge the target can get before the arrow returns to the edge.")]
    [Range(0f, 0.45f)]
    [SerializeField] private float m_cameraExitThreshold = 0f;


    [Header("Movement Settings")]
    [Tooltip("How quickly the arrow moves into position.")]
    [Min(0f)]
    [SerializeField] private float m_positionSmoothSpeed = 8f;

    [Tooltip("How long the arrow takes to transition between the screen edge and target.")]
    [Min(0.01f)]
    [SerializeField] private float m_positionTransitionDuration = 0.25f;

    [Tooltip("How quickly the arrow rotates into position.")]
    [Min(0f)]
    [SerializeField] private float m_rotationSmoothSpeed = 8f;


    [Header("Distance Scale Settings")]
    [Tooltip("Distance at which the arrow reaches its maximum size, in world units.")]
    [Min(0f)]
    [SerializeField] private float m_nearScaleDistance = 3f;

    [Tooltip("Distance at which the arrow reaches its minimum size, in world units.")]
    [Min(0f)]
    [SerializeField] private float m_farScaleDistance = 20f;

    [Tooltip("Smallest arrow scale.")]
    [Min(0f)]
    [SerializeField] private float m_minScale = 0.7f;

    [Tooltip("Largest arrow scale.")]
    [Min(0f)]
    [SerializeField] private float m_maxScale = 1.2f;

    [Tooltip("How quickly the arrow changes size.")]
    [Min(0f)]
    [SerializeField] private float m_scaleSmoothSpeed = 8f;


    [Header("Animation Settings")]
    [Tooltip("Amount the off-screen arrow pulses.")]
    [Range(0f, 0.1f)]
    [SerializeField] private float m_pulseAmount = 0.04f;

    [Tooltip("Speed of the off-screen arrow pulse.")]
    [Min(0f)]
    [SerializeField] private float m_pulseSpeed = 2.5f;


    private RectTransform m_rectTransform;
    private Canvas m_canvas;

    private Camera m_camera;
    private Transform m_target;
    private Renderer m_targetRenderer;

    private Vector3 m_baseScale;
    private float m_baseRotation;

    private Vector2 m_currentPosition;
    private Vector2 m_transitionStartPosition;
    private float m_currentRotation;
    private Vector3 m_currentScale;

    private float m_transitionProgress;

    private bool m_isAboveTarget;
    private bool m_isPositionTransitioning;
    private bool m_isInitialized;


    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();

        Canvas canvas = GetComponentInParent<Canvas>();
        m_canvas = canvas != null ? canvas.rootCanvas : null;

        m_baseScale = m_rectTransform.localScale;
        m_baseRotation = m_rectTransform.localEulerAngles.z;
    }


    private void LateUpdate()
    {
        if (m_camera == null || m_target == null)
            return;

        Vector3 targetWorldPosition = GetTargetWorldPosition();

        Vector3 viewportPosition =
            m_camera.WorldToViewportPoint(targetWorldPosition);

        UpdateArrowState(viewportPosition);

        Vector2 targetScreenPosition;
        float targetRotation;

        if (m_isAboveTarget)
        {
            Vector3 targetTopPosition = m_targetRenderer != null
                ? m_targetRenderer.bounds.center +
                  Vector3.up * m_targetRenderer.bounds.extents.y
                : targetWorldPosition;

            targetScreenPosition =
                m_camera.WorldToScreenPoint(targetTopPosition);

            targetScreenPosition.y += m_targetScreenOffset;

            targetRotation = m_baseRotation;
        }
        else
        {
            GetEdgePosition(
                viewportPosition,
                targetWorldPosition,
                out targetScreenPosition,
                out targetRotation);
        }

        targetScreenPosition = ClampToCameraRect(targetScreenPosition);

        float targetScale = GetDistanceScale();

        if (!m_isAboveTarget)
        {
            targetScale *= 1f +
                Mathf.Sin(Time.unscaledTime * m_pulseSpeed) *
                m_pulseAmount;
        }

        Vector3 targetScaleVector = m_baseScale * targetScale;

        if (!TryGetLocalPosition(
                targetScreenPosition,
                out Vector2 targetLocalPosition))
        {
            return;
        }

        if (!m_isInitialized)
        {
            m_currentPosition = targetLocalPosition;
            m_currentRotation = targetRotation;
            m_currentScale = targetScaleVector;

            m_isInitialized = true;
        }
        else
        {
            if (m_isPositionTransitioning)
            {
                m_transitionProgress +=
                    Time.deltaTime / m_positionTransitionDuration;

                float progress = Mathf.SmoothStep(
                    0f,
                    1f,
                    Mathf.Clamp01(m_transitionProgress));

                m_currentPosition = Vector2.Lerp(
                    m_transitionStartPosition,
                    targetLocalPosition,
                    progress);

                if (m_transitionProgress >= 1f)
                {
                    m_currentPosition = targetLocalPosition;
                    m_isPositionTransitioning = false;
                }
            }
            else if (m_isAboveTarget)
            {
                m_currentPosition = targetLocalPosition;
            }
            else
            {
                m_currentPosition = Vector2.Lerp(
                    m_currentPosition,
                    targetLocalPosition,
                    GetSmoothFactor(m_positionSmoothSpeed));
            }

            m_currentRotation = Mathf.LerpAngle(
                m_currentRotation,
                targetRotation,
                GetSmoothFactor(m_rotationSmoothSpeed));

            m_currentScale = Vector3.Lerp(
                m_currentScale,
                targetScaleVector,
                GetSmoothFactor(m_scaleSmoothSpeed));
        }

        m_rectTransform.anchoredPosition = m_currentPosition;

        m_rectTransform.localRotation =
            Quaternion.Euler(0f, 0f, m_currentRotation);

        m_rectTransform.localScale = m_currentScale;
    }


    /// <summary>
    /// Initializes the arrow with the camera and target it should follow.
    /// </summary>
    /// <param name="_camera">Camera used to calculate arrow positioning.</param>
    /// <param name="_target">Transform of the quest destination.</param>
    public void Initialize(Camera _camera, Transform _target)
    {
        m_camera = _camera;
        m_target = _target;

        m_targetRenderer = _target != null
            ? _target.GetComponentInChildren<Renderer>()
            : null;

        m_isAboveTarget = false;
        m_isPositionTransitioning = false;
        m_isInitialized = false;
    }


    private Vector3 GetTargetWorldPosition()
    {
        return m_targetRenderer != null
            ? m_targetRenderer.bounds.center
            : m_target.position;
    }


    private void UpdateArrowState(Vector3 _viewportPosition)
    {
        float threshold = m_isAboveTarget
            ? m_cameraExitThreshold
            : m_cameraEntryThreshold;

        bool wasAboveTarget = m_isAboveTarget;

        m_isAboveTarget = IsInsideCameraView(
            _viewportPosition,
            threshold);

        if (m_isInitialized && wasAboveTarget != m_isAboveTarget)
        {
            m_transitionStartPosition = m_currentPosition;
            m_transitionProgress = 0f;
            m_isPositionTransitioning = true;
        }
    }


    private bool IsInsideCameraView(
        Vector3 _position,
        float _threshold)
    {
        return _position.z > 0f &&
               _position.x >= _threshold &&
               _position.x <= 1f - _threshold &&
               _position.y >= _threshold &&
               _position.y <= 1f - _threshold;
    }


    private void GetEdgePosition(
        Vector3 _viewportPosition,
        Vector3 _targetWorldPosition,
        out Vector2 _screenPosition,
        out float _rotation)
    {
        Rect cameraRect = m_camera.pixelRect;

        Vector3 targetPosition =
            m_camera.WorldToScreenPoint(_targetWorldPosition);

        bool targetIsLeft = _viewportPosition.x < 0.5f;

        float edgeX = targetIsLeft
            ? cameraRect.xMin + m_edgePadding
            : cameraRect.xMax - m_edgePadding;

        float edgeY = Mathf.Clamp(
            targetPosition.y,
            cameraRect.yMin + m_edgePadding,
            cameraRect.yMax - m_edgePadding);

        _screenPosition = new Vector2(edgeX, edgeY);

        _rotation = m_baseRotation + (targetIsLeft ? -90f : 90f);
    }


    private Vector2 ClampToCameraRect(Vector2 _position)
    {
        Rect rect = m_camera.pixelRect;

        float minX = rect.xMin + m_edgePadding;
        float maxX = rect.xMax - m_edgePadding;
        float minY = rect.yMin + m_edgePadding;
        float maxY = rect.yMax - m_edgePadding;

        if (minX > maxX)
            minX = maxX = rect.center.x;

        if (minY > maxY)
            minY = maxY = rect.center.y;

        return new Vector2(
            Mathf.Clamp(_position.x, minX, maxX),
            Mathf.Clamp(_position.y, minY, maxY));
    }


    private float GetDistanceScale()
    {
        float distance = Vector2.Distance(
            m_camera.transform.position,
            m_target.position);

        float nearDistance = Mathf.Max(0f, m_nearScaleDistance);
        float farDistance = Mathf.Max(
            nearDistance + 0.01f,
            m_farScaleDistance);

        return Mathf.Lerp(
            m_maxScale,
            m_minScale,
            Mathf.InverseLerp(nearDistance, farDistance, distance));
    }


    private bool TryGetLocalPosition(
        Vector2 _screenPosition,
        out Vector2 _localPosition)
    {
        RectTransform parentRect =
            m_rectTransform.parent as RectTransform;

        if (parentRect == null)
        {
            _localPosition = Vector2.zero;
            return false;
        }

        Camera eventCamera = m_canvas == null ||
                             m_canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : m_canvas.worldCamera != null
                ? m_canvas.worldCamera
                : m_camera;

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            _screenPosition,
            eventCamera,
            out _localPosition);
    }


    private float GetSmoothFactor(float _speed)
    {
        return _speed <= 0f
            ? 1f
            : 1f - Mathf.Exp(-_speed * Time.deltaTime);
    }
}
