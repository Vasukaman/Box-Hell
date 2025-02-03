using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConnectionHelper
{
    public static void AlignTransforms(Transform source, Transform target)
    {
        // Calculate rotation difference
        Quaternion rotDifference = target.rotation * Quaternion.Inverse(source.rotation);

        // Apply rotation
        source.rotation = rotDifference * source.rotation;

        // Calculate position offset
        Vector3 posOffset = target.position - source.position;
        source.position += posOffset;
    }
}