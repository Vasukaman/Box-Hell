using System.Collections;
using UnityEngine;

public class ProceduralWeaponAnimator : MonoBehaviour
{

    public enum SwingState { Resting, Preparing, Attacking, Pausing, Returning }

    [Header("References")]
    public Transform weaponTransform;
    public Camera playerCamera;
    public Transform preparePoint;
    public Transform restingPoint;
    public Transform finalHitTransform;
    [SerializeField] private RegularMeleeTool regularMeleeTool;


    [Header("Animation Settings")]
    public AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float prepareDuration = 0.2f;
    public float attackDuration = 0.3f;
    public float returnDuration = 0.2f;
    public float postHitPause = 0.1f;
    public float attackDistance = 3f;
    public float arcPower = 1f;
    [Range(0, 360)] public float arcAngle = 0f;

    private SwingState currentState = SwingState.Resting;
    private Vector3 currentHitPoint;
    private float stateProgress;
    private Vector3[] attackArcPoints = new Vector3[3];

    [Header("Debug")]
    public bool debugMode;
    public DebugPose debugPreview = DebugPose.Resting;
    [Range(0, 1)] public float debugArcPreview = 1f;

    public enum DebugPose { Resting, Prepare, Hit, FinalHitTransform }
    private void Start()
    {
        playerCamera = Camera.main;
    }
    void Update()
    {


        switch (currentState)
        {
            case SwingState.Preparing:
                UpdatePreparation();
                break;
            case SwingState.Attacking:
                UpdateAttack();
                break;
            case SwingState.Pausing:
                UpdatePause();
                break;
            case SwingState.Returning:
                UpdateReturn();
                break;
        }


        if (debugMode)
        {
            UpdateDebugPreview();
            return;
        }
    }

   public void StartSwing()
    {
        currentState = SwingState.Preparing;
        stateProgress = 0f;
    }

    void UpdatePreparation()
    {
        stateProgress += Time.deltaTime / prepareDuration;
        weaponTransform.position = Vector3.Lerp(restingPoint.position, preparePoint.position,
            swingCurve.Evaluate(stateProgress));
        weaponTransform.rotation = Quaternion.Slerp(restingPoint.rotation, preparePoint.rotation,
            swingCurve.Evaluate(stateProgress));

        if (stateProgress >= 1f)
        {
            CalculateAttackArc();
            currentState = SwingState.Attacking;
            stateProgress = 0f;
        }
    }

    void CalculateAttackArc()
    {
        // Raycast to find hit point
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        currentHitPoint = Physics.Raycast(ray, out hit, attackDistance) ?
            hit.point :
            ray.GetPoint(attackDistance);

        // Calculate arc points
        Vector3 startPos = preparePoint.position;
        Vector3 endPos = currentHitPoint + finalHitTransform.localPosition;

        // Calculate arc direction based on angle
        Vector3 cameraForward = playerCamera.transform.forward;
        Quaternion angleRotation = Quaternion.AngleAxis(arcAngle, cameraForward);
        Vector3 arcDirection = angleRotation * playerCamera.transform.up;

        attackArcPoints[0] = startPos;
        attackArcPoints[1] = Vector3.Lerp(startPos, endPos, 0.5f) + arcDirection * arcPower;
        attackArcPoints[2] = endPos;
    }

    void UpdateAttack()
    {
        stateProgress += Time.deltaTime / attackDuration;
        float curvedProgress = swingCurve.Evaluate(stateProgress);

        // Quadratic Bezier curve for arc movement
        weaponTransform.position = CalculateBezierPoint(curvedProgress);
        weaponTransform.rotation = Quaternion.Slerp(
            preparePoint.rotation,
            finalHitTransform.rotation,
            curvedProgress
        );

        if (stateProgress >= 1f)
        {
            Damage();               
            currentState = SwingState.Pausing;
            stateProgress = 0f;
        }
    }

    Vector3 CalculateBezierPoint(float t)
    {
        float u = 1 - t;
        return u * u * attackArcPoints[0] +
               2 * u * t * attackArcPoints[1] +
               t * t * attackArcPoints[2];
    }

    void UpdatePause()
    {
        stateProgress += Time.deltaTime;
        if (stateProgress >= postHitPause)
        {
            currentState = SwingState.Returning;
            stateProgress = 0f;
        }
    }

    void UpdateReturn()
    {
        stateProgress += Time.deltaTime / returnDuration;
        weaponTransform.position = Vector3.Lerp(
            attackArcPoints[2],
            restingPoint.position,
            swingCurve.Evaluate(stateProgress)
        );
        weaponTransform.rotation = Quaternion.Slerp(
            finalHitTransform.rotation,
            restingPoint.rotation,
            swingCurve.Evaluate(stateProgress)
        );

        if (stateProgress >= 1f)
        {
            currentState = SwingState.Resting;
        }
    }
        

    void UpdateDebugPreview()
    {
        switch (debugPreview)
        {
            case DebugPose.Resting:
                weaponTransform.position = restingPoint.position;
                weaponTransform.rotation = restingPoint.rotation;
                break;

            case DebugPose.Prepare:
                weaponTransform.position = preparePoint.position;
                weaponTransform.rotation = preparePoint.rotation;
                break;

            case DebugPose.Hit:
                CalculateAttackArc();
                weaponTransform.position = attackArcPoints[2];
                weaponTransform.rotation = finalHitTransform.rotation;
                break;

            case DebugPose.FinalHitTransform:
                CalculateAttackArc();
                weaponTransform.position = currentHitPoint + finalHitTransform.localPosition;
                weaponTransform.rotation = finalHitTransform.rotation;
                break;
        }
    }

    void OnDrawGizmos()
    {
        if (!debugMode) return;

        // Draw all important points
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(restingPoint.position, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(preparePoint.position, 0.1f);

        if (Application.isPlaying)
        {
            CalculateAttackArc();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackArcPoints[2], 0.15f);
            Gizmos.DrawWireSphere(currentHitPoint, 0.1f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(currentHitPoint + finalHitTransform.localPosition, 0.15f);

            // Preview arc
            if (attackArcPoints != null && attackArcPoints.Length == 3)
            {
                Gizmos.color = Color.cyan;
                Vector3 prevPoint = attackArcPoints[0];
                for (int i = 1; i <= 20; i++)
                {
                    float t = i / 20f * debugArcPreview;
                    Vector3 point = CalculateBezierPoint(t);
                    Gizmos.DrawLine(prevPoint, point);
                    prevPoint = point;
                }
            }
        }


        if (Application.isPlaying && attackArcPoints != null && attackArcPoints.Length == 3)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(attackArcPoints[0], attackArcPoints[1]);
            Gizmos.DrawLine(attackArcPoints[1], attackArcPoints[2]);
        }

    }

    void Damage()
    {
        regularMeleeTool.TryAttacking();
    }
}