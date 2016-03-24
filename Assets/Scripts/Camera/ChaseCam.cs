using UnityEngine;
using System.Collections;

public class ChaseCam : MonoBehaviour
{
    public enum UpdateMode
    {
        Update, FixedUpdate, LateUpdate
    }

    public Transform Pivot;
    public Camera Cam;
    public CarMovement Target;
    public float FollowSmoothTime;
    public float RotateSmoothTime;
    public float Height, Depth, Angle;
    public bool Follow;
    public UpdateMode UpdateType;

    void Start()
    {
        StartCoroutine(UpdateParameters());
    }

    void Update()
    {
        if (Follow && UpdateType == UpdateMode.Update)
        {
            UpdateCameraPosition(Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (Follow && UpdateType == UpdateMode.FixedUpdate)
        {
            UpdateCameraPosition(Time.fixedDeltaTime);
        }
    }

    void LateUpdate()
    {
        if (Follow && UpdateType == UpdateMode.LateUpdate)
        {
            UpdateCameraPosition(Time.deltaTime);
        }
    }

    void UpdateCameraPosition(float timeMultiplier)
    {
        Pivot.rotation = Quaternion.Lerp(Pivot.rotation, Target.transform.rotation, timeMultiplier * RotateSmoothTime);
        Pivot.position = Vector3.Lerp(Pivot.position, Target.transform.position, timeMultiplier * FollowSmoothTime);
    }

    IEnumerator UpdateParameters()
    {
        var height = -1f;
        var depth = -1f;
        var angle = -1f;

        while (true)
        {
            if (Height != height || Depth != depth || Angle != angle)
            {
                Cam.transform.localPosition = new Vector3(0, Height, Depth);
                var euler = Cam.transform.eulerAngles;
                euler.x = Angle;
                Cam.transform.rotation = Quaternion.Euler(euler);
                height = Height;
                depth = Depth;
                angle = Angle;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
