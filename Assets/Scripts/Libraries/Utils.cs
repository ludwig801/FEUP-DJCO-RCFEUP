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

    public static Vector3 ProjectVector3(Vector3 a, Vector3 b)
    {
        return (Vector3.Dot(a,b) * a / a.sqrMagnitude);
    }

    public static Vector3 ProjectVector3OnlyXZ(Vector3 a, Vector3 b, float percentage)
    {
        var projected = Vector3.Dot(a, b) * a / a.sqrMagnitude;
        return new Vector3(projected.x, b.y, projected.z);
    }

    public static string GetCounterFormattedString(float time)
    {
        var span = System.TimeSpan.FromSeconds(time);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Minutes, span.Seconds, (span.Milliseconds % 100));
    }

    public static string GetOrdinalEnding(int value)
    {
        if (value > 3 && value < 21)
        {
            return "TH";
        }
        else
        {
            var lastDigit = GetRightmostDigit(value);

            if (lastDigit == 1)
            {
                return "ST";
            }
            else if (lastDigit == 2)
            {
                return "ND";
            }
            else if (lastDigit == 3)
            {
                return "RD";
            }
        }

        return "TH";
    }

    public static int GetRightmostDigit(int value)
    {
        return value % 10;
    }

    public static bool IsColorLike(Color a, Color b, float toleranceFactor = 0.01f)
    {
        var deltaR = Mathf.Abs(a.r - b.r);
        var deltaG = Mathf.Abs(a.g - b.g);
        var deltaB = Mathf.Abs(a.b - b.b);
        var deltaA = Mathf.Abs(a.a - b.a);

        return deltaR < toleranceFactor &&
            deltaG < toleranceFactor &&
            deltaB < toleranceFactor &&
            deltaA < toleranceFactor;
    }
}
