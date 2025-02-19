using UnityEngine;
using DG.Tweening;

public class TwoPointMover : MonoBehaviour
{
    [Header("References")]
    public Transform pointA;
    public Transform pointB;

    [Header("Timing")]
    public float moveDuration = 1f;
    public float returnDelay = 2f;

    private enum State { Inactive, MovingToTarget, Waiting, MovingBack }
    private State currentState = State.Inactive;
    private Tween currentTween;

    public void Activate()
    {
        // Prevent multiple activations when already at target position
        if (currentState == State.MovingToTarget ||
            currentState == State.Waiting) return;

        // Cancel any ongoing movement
        currentTween?.Kill();

        currentState = State.MovingToTarget;

        currentTween = transform.DOMove(pointB.position, moveDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => {
                currentState = State.Waiting;
                StartCoroutine(ReturnAfterDelay());
            });
    }

    private System.Collections.IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(returnDelay);

        currentState = State.MovingBack;

        currentTween = transform.DOMove(pointA.position, moveDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => currentState = State.Inactive);
    }

    // Add safety checks
    private void OnValidate()
    {
        if (pointA == null || pointB == null)
            Debug.LogWarning($"Assign both points in {gameObject.name}!", this);
    }

    // Reset position on disable
    private void OnDisable()
    {
        currentTween?.Kill();
        transform.position = pointA.position;
        currentState = State.Inactive;
    }
}

