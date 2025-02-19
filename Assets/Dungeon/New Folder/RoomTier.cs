using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public struct RoomPackConfiguration
{
  public  RoomPackSO roomPack;
    public int minNumberToSPawn;
    public int maxNumberToSPawn;
}
[CreateAssetMenu(menuName = "Dungeon/Room Tier")]
public class RoomTier : ScriptableObject
{
    public int tier;
    public List<RoomPackConfiguration> roomConfigurations;
    public List<CorridorConfiguration> corridorConfigurations;
}