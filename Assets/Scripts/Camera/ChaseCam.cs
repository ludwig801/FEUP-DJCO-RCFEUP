using UnityEngine;

public class ChaseCam : MonoBehaviour
{
    public Transform Pivot;
    public Camera Cam;
    public Transform Target;
    public float FollowSmoothTime;
    public float RotateSmoothTime;
    public float Height, Depth;
    public float InclinationAngle;

    void Update()
    {
        Cam.transform.localPosition = new Vector3(0, Height, Depth);
    }

    void LateUpdate()
    {
        Pivot.rotation = Quaternion.Lerp(Pivot.rotation, Target.rotation, Time.deltaTime * RotateSmoothTime);
        Pivot.position = Vector3.Lerp(Pivot.position, Target.position, Time.deltaTime * FollowSmoothTime);
        
    }
}
