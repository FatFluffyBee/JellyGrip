
using UnityEngine;

public static class VectorHelpers
{
    /// <summary>
    /// Flattens the vector in XY then normalize it.
    /// </summary>
    /// <param name="v">The source vector.</param>
    /// 
    public static void ToV2Dir (ref this Vector3 v)
    {
        v.z = 0f;
        v.Normalize();
    } 
}
