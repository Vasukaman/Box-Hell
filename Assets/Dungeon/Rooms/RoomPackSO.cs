using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Dungeon/RooPack")]
public class RoomPackSO : ScriptableObject
{
    public List<RoomConfiguration> roomConfigurations;
}
