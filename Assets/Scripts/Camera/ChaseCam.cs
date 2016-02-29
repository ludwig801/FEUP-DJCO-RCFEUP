using UnityEngine;
using System.Collections;

public class ChaseCam : MonoBehaviour
{
    public Transform Target;
    public float SmoothTime;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x, transform.position.y, Target.position.z), Time.deltaTime * SmoothTime);
    }
}
