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

    public static Vector3 DivideVector3(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static string GetCounterFormattedString(float time)
    {
        var span = System.TimeSpan.FromSeconds(time);
        return string.Format("{0:00}:{1:00}:{2:00}", span.Minutes, span.Seconds, (span.Milliseconds % 100));
    }
}
