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


    private Vector3 finalHitPosition;
    private Quaternion finalHitRotation;
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
        // Get camera transform reference
        Transform camTransform = playerCamera.transform;

        // Raycast using camera's forward
        Ray ray = new Ray(camTransform.position, camTransform.forward);
        RaycastHit hit;
        currentHitPoint = Physics.Raycast(ray, out hit, attackDistance) ?
            hit.point :
            ray.GetPoint(attackDistance);

        // Calculate final position in WORLD space using camera rotation
        Vector3 finalOffset = camTransform.TransformPoint(finalHitTransform.localPosition);
        Vector3 finalPosition = currentHitPoint + (finalOffset - camTransform.position);

        // Calculate arc direction based on camera-relative angle
        Vector3 arcNormal = camTransform.forward;
        Vector3 arcDirection = Quaternion.AngleAxis(arcAngle, arcNormal) * camTransform.up;

        // Calculate arc points using camera-oriented positions
        attackArcPoints[0] = preparePoint.position;
        attackArcPoints[1] = Vector3.Lerp(preparePoint.position, finalPosition, 0.5f)
                             + arcDirection * arcPower;
        attackArcPoints[2] = finalPosition;

        // Store final rotation with camera-relative adjustment
        finalHitTransform.rotation = Quaternion.LookRotation(
            camTransform.forward,
            finalHitTransform.up
        ) * finalHitTransform.localRotation;
    }

    void UpdateAttack()
    {

        stateProgress += Time.deltaTime / attackDuration;
        float curvedProgress = swingCurve.Evaluate(stateProgress);

        weaponTransform.position = CalculateBezierPoint(curvedProgress);
        weaponTransform.rotation = Quaternion.Slerp(
            preparePoint.rotation,
            finalHitRotation,
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
                weaponTransform.position = finalHitPosition;
                weaponTransform.rotation = finalHitRotation;
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