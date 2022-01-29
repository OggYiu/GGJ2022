using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
    public static float BudColliderZ = 0f;

    public static bool Equals(float value, float target)
    {
        return Mathf.Abs(value - target) <= Mathf.Epsilon;
    }
    public static bool IsZero(float value)
    {
        return Mathf.Abs(value) <= Mathf.Epsilon;
    }
    public static bool IsZero(Vector2 vec)
    {
        return IsZero(vec.x) && IsZero(vec.y);
    }
}
