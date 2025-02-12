using UnityEngine;
using System.Collections.Generic;

public enum RoomState { Stable, Preparation, Incineration }

[CreateAssetMenu(menuName = "Settings/Room State Settings")]
public class RoomStateSettings : ScriptableObject
{
    public float preparationTime = 10f;
    public float incinerationTime = 20f;
    public float minStartTime = 2f;
    public float maxStartTime = 5f;
    public float damagePause = 1f;
}