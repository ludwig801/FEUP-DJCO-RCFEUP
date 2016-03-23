using System.Collections.Generic;
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

    public enum WindowPositions
    {
        Center, Right, Up, Down, Left
    }

    public static void GetAnchorsForWindowPosition(WindowPositions targetPosition, out Vector2 anchorMin, out Vector2 anchorMax)
    {
        switch (targetPosition)
        {
            case WindowPositions.Right:
                anchorMin = Vector2.right;
                anchorMax = new Vector2(2, 1);
                break;

            case WindowPositions.Up:
                anchorMin = Vector2.up;
                anchorMax = new Vector2(1, 2);
                break;

            case WindowPositions.Down:
                anchorMin = new Vector2(0, -1);
                anchorMax = Vector2.right;
                break;

            case WindowPositions.Left:
                anchorMin = new Vector2(-1, 0);
                anchorMax = Vector2.up;
                break;

            default:
                anchorMin = Vector2.zero;
                anchorMax = Vector2.one;
                break;
        }
    }

    public static bool LerpRectTransformAnchors(RectTransform rectTransform, Vector2 targetMin, Vector2 targetMax, float speed)
    {
        rectTransform.anchorMin = Vector2.Lerp(rectTransform.anchorMin, targetMin, speed);
        rectTransform.anchorMax = Vector2.Lerp(rectTransform.anchorMax, targetMax, speed);

        if (Vector2.Distance(rectTransform.anchorMin, targetMin) < 0.001f || Vector2.Distance(rectTransform.anchorMax, targetMax) < 0.001f)
        {
            rectTransform.anchorMin = targetMin;
            rectTransform.anchorMax = targetMax;
            return true;
        }

        return false;
    }

    public static List<Transform> ShuffleList(List<Transform> list)
    {
        System.Random rand = new System.Random();
        Transform obj;

        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            int r = i + (int)(rand.NextDouble() * (n - i));
            obj = list[r];
            list[r] = list[i];
            list[i] = obj;
        }

        return list;
    }
}
