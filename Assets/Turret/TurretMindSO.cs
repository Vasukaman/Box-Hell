using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/Turret Settings")]
public class TurretMindSO : ScriptableObject
{
    [Header("Detection")]
    public float detectionRange = 30f;
    public float fovAngle = 90f;
    public LayerMask obstructionLayers;

    [Header("Rotation Limits")]
    public float minHorizontalAngle = -45f;
    public float maxHorizontalAngle = 45f;
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 30f;

    [Header("Idle Behavior")]
    public Vector2 idlePointA = new Vector2(-30f, 0f);
    public Vector2 idlePointB = new Vector2(30f, 0f);
    public float idleRotationSpeed = 30f;

    [Header("Combat")]
    public float aimSpeed = 45f;
    public float aimThreshold = 5f;
    public float shootCooldown = 0.5f;
    public float timeBeforeToPrepareToShoot=0.4f;

    [Header("First Shot")]
    public float initialShotDelay = 0.5f; 
}