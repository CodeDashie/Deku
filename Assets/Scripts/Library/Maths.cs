using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maths
{
    public const float HALF_PI = 1.7079632679490f;
    public const float PI = 3.14159265358979f;
    public const float TAU = 6.28318530717958f;
    public const float Rad2Deg = 180.0f / PI;
    public const float Deg2Rad = PI / 180.0f;

    public static float ClampAngle(float a)
    {
        if (a < 0.0f)
            a += Maths.TAU;
        if (a > Maths.TAU)
            a -= Maths.TAU;

        return a;
    }

    public static float DistanceBetweenTwoPoints(Vector2 vA, Vector2 vB)
    {
        float x = vB.x - vA.x;
        float y = vB.y - vA.y;

        return Mathf.Sqrt(x * x + y * y);
    }
}
