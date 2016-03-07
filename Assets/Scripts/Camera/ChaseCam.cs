using UnityEngine;

public class ChaseCam : MonoBehaviour
{
    public Transform Pivot;
    public Camera Cam;
    public Transform Target;
    public float SmoothTime;
    public float DistanceToTarget;
    public float InclinationAngle;

    [SerializeField]
    float _height;
    [SerializeField]
    float _depth;
    float _oldDistance;

    void Start()
    {
        _oldDistance = -1;
    }

    void LateUpdate()
    {
        if (_oldDistance != DistanceToTarget)
        {
            _depth = -Mathf.Sin(45) * DistanceToTarget;
            _height = Mathf.Cos(45) * DistanceToTarget;
            _oldDistance = DistanceToTarget;
        }

        Pivot.rotation = Quaternion.Lerp(Pivot.rotation, Target.rotation, Time.deltaTime * SmoothTime);
        Pivot.position = Target.position;
        Cam.transform.localPosition = new Vector3(0, _height, _depth);
    }
}
