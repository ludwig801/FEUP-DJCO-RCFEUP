using UnityEngine;

public abstract class Utils
{
    public static float ClampRotation(float value, float spread)
    {
        if (value <= 180)
            return Mathf.Min(value, spread);
        else
            return Mathf.Max(value, 360 - spread);
    }
}
