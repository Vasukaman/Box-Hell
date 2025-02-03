using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dungeon/Room Tier")]
public class RoomTier : ScriptableObject
{
    public int tier;
    public List<RoomConfiguration> roomConfigurations;
    public List<CorridorConfiguration> corridorConfigurations;
}