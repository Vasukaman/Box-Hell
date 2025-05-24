using UnityEngine;
using DG.Tweening;

public class TwoPointMover : MonoBehaviour
{
    public enum MovementMode
    {
        PingPong,    // Original behavior: A->B->A
        Toggle       // New behavior: A->B, then B->A on next activation
    }

    [Header("References")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public MovementMode mode = MovementMode.PingPong;
    public float moveDuration = 1f;
    public float returnDelay = 2f;

    [Header("Toggle Mode Settings")]
    [Tooltip("Starting position for toggle mode")]
    public bool startAtPointA = true;

    private State currentState = State.Inactive;
    private Tween currentTween;
    private Transform currentTarget;
    private Transform nextTarget;

    private enum State { Inactive, Moving, Waiting }

    private void Start()
    {
        InitializeTargets();
    }

    private void InitializeTargets()
    {
        currentTarget = startAtPointA ? pointB : pointA;
        nextTarget = startAtPointA ? pointA : pointB;
    }

    public void Activate()
    {
        if (currentState != State.Inactive) return;

        currentState = State.Moving;

        if (mode == MovementMode.Toggle)
        {
            MoveToTarget(currentTarget.position, () =>
            {
                (currentTarget, nextTarget) = (nextTarget, currentTarget);
                currentState = State.Inactive;
            });
        }
        else
        {
            MoveToTarget(pointB.position, () =>
            {
                currentState = State.Waiting;
                StartCoroutine(ReturnAfterDelay());
            });
        }
    }

    private void MoveToTarget(Vector3 target, TweenCallback onComplete)
    {
        currentTween?.Kill();
        currentTween = transform.DOMove(target, moveDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(onComplete);
    }

    private System.Collections.IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(returnDelay);

        MoveToTarget(pointA.position, () =>
        {
            currentState = State.Inactive;
        });
    }

    // Add safety checks
    private void OnValidate()
    {
        if (pointA == null || pointB == null)
            Debug.LogWarning($"Assign both points in {gameObject.name}!", this);
    }

    private void OnDisable()
    {
        currentTween?.Kill();
        transform.position = startAtPointA ? pointA.position : pointB.position;
        currentState = State.Inactive;
        InitializeTargets();
    }
}