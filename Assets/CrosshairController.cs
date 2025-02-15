using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    // Crosshair states

    [SerializeField] private HittableDetector _hittableDetector;
    private enum CrosshairState { Dot, Circle }
    private CrosshairState currentState;

    // Components for rendering
    public LineRenderer circularIndicatorRenderer;
    public Transform centerPoint;

    // Drawing parameters
    public int circleSteps = 100;
    public int dotSteps = 32;
    public float minCircleRadius = 10f;
    public float maxCircleRadius = 20f;
    public float dotRadius = 2f;
    public float dotWidth = 0.2f;
    public float circleWidth = 0.1f;


    private void OnEnable()
    {
        _hittableDetector.OnHittableDetected += OnHittableFound;
        _hittableDetector.OnHittableLost += OnHittableLost;
    }

    private void OnDisable()
    {
        _hittableDetector.OnHittableDetected -= OnHittableFound;
        _hittableDetector.OnHittableLost -= OnHittableLost;
    }

    private void OnHittableFound(IHittable hittable, float distance)
    {
        float maxRange = _hittableDetector.currentRange;
        UpdateCrosshair(true, distance, maxRange);
    }

    private void OnHittableLost()
    {
        UpdateCrosshair(false, 0, 0);
    }

    private void Start()
    {
        // Initialize the dot
        drawCircularIndicator(dotSteps, dotRadius, dotWidth);
        circularIndicatorRenderer.gameObject.SetActive(true);
    }


    public void UpdateCrosshair(bool isLookingAtObject, float distanceToObj, float maxDistance)
    {
        // Update crosshair state based on whether we have a target
        if (isLookingAtObject && distanceToObj <= maxDistance)
        {
            currentState = CrosshairState.Circle;
        }
        else
        {
            currentState = CrosshairState.Dot;
        }

        // Update the crosshair visuals based on the current state
        switch (currentState)
        {
            case CrosshairState.Dot:
                drawCircularIndicator(dotSteps, dotRadius, dotWidth);
                break;
            case CrosshairState.Circle:
                float radius = Mathf.Lerp(minCircleRadius, maxCircleRadius, 1 -(distanceToObj / maxDistance));
                drawCircularIndicator(circleSteps, radius, circleWidth);
                break;
        }
    }

    private void drawCircularIndicator(int steps, float radius, float width)
    {
        if (circularIndicatorRenderer == null)
        {
            Debug.LogError("CircularIndicatorRenderer is not assigned in the inspector!");
            return;
        }

        circularIndicatorRenderer.positionCount = steps;
        circularIndicatorRenderer.startWidth = width;
        circularIndicatorRenderer.endWidth = width;

        for (int i = 0; i < steps; i++)
        {
            float progress = (float)i / (steps - 1);
            float radian = progress * 2 * Mathf.PI;

            float x = radius * Mathf.Cos(radian);
            float y = radius * Mathf.Sin(radian);

            Vector3 position = centerPoint.localPosition + new Vector3(x, y, 0);
            circularIndicatorRenderer.SetPosition(i, position);
        }
    }
}