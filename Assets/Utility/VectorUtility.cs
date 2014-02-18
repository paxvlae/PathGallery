using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public static class VectorUtility {
    public static Vector3 InvertY(this Vector3 pos)
    {
        return new Vector3(pos.x, Screen.height - pos.y, pos.z);
    }
    public static Vector2 InvertY(this Vector2 pos)
    {
        return new Vector2(pos.x, Screen.height - pos.y);
    }
    
    static public bool EqualWithEpsilon(this Vector3 actual, Vector3 target, Vector3 epsilon)
    {
        if ((actual.magnitude >= (target - epsilon).magnitude) &&
            (actual.magnitude <= (target + epsilon).magnitude))
            return true;
        else
            return false;
    }
    
    static public bool EqualWithEpsilon(this Vector2 actual, Vector2 target, Vector2 epsilon)
    {
        if ((actual.magnitude >= (target - epsilon).magnitude) &&
            (actual.magnitude <= (target + epsilon).magnitude))
            return true;
        else
            return false;
    }
    public static Vector2 VectorAverange(this IEnumerable<Vector2> collection)
    {
        var enumerable = collection as Vector2[] ?? collection.ToArray();
        Vector2 sum = enumerable.Aggregate(Vector2.zero, (current, val) => current + val);
        var vectorCollection = collection as ICollection<Vector2>;
        int count = vectorCollection != null ? vectorCollection.Count : enumerable.Count();
        return sum / count;
    }
    public static Vector3 VectorAverange(this IEnumerable<Vector3> collection)
    {
        var enumerable = collection as Vector3[] ?? collection.ToArray();
        Vector3 sum = enumerable.Aggregate(Vector3.zero, (current, val) => current + val);
        var vectorCollection = collection as ICollection<Vector3>;
        int count = vectorCollection != null ? vectorCollection.Count : enumerable.Count();
        return sum / count;
    }
}
