using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ConnectionGizmos : MonoBehaviour
{
  //  void OnDrawGizmos()
    //{
    //    var room = GetComponent<RoomManager>();
    //    if (room == null) return;

    //    Gizmos.color = Color.green;
    //    foreach (var entry in room.entryPoints)
    //    {
    //        if (entry.spawnAnchor != null)
    //        {
    //            Gizmos.DrawSphere(entry.spawnAnchor.position, 0.2f);
    //            Gizmos.DrawLine(entry.spawnAnchor.position,
    //                entry.spawnAnchor.position + entry.spawnAnchor.forward * 1f);
    //        }
    //    }

    //    Gizmos.color = Color.red;
    //    foreach (var exit in room.exitPoints)
    //    {
    //        if (exit.spawnAnchor != null)
    //        {
    //            Gizmos.DrawSphere(exit.spawnAnchor.position, 0.2f);
    //            Gizmos.DrawLine(exit.spawnAnchor.position,
    //                exit.spawnAnchor.position + exit.spawnAnchor.forward * 1f);
    //        }
    //    }
    //}
}
