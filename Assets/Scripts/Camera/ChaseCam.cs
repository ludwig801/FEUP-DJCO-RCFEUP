using UnityEngine;
using System.Collections;

public class ChaseCam : MonoBehaviour
{
    public Transform Pivot;
    public Camera Cam;
    public Transform Target;
    public float FollowSmoothTime;
    public float RotateSmoothTime;
    public float Height, Depth;
    public bool Follow;

    void Start()
    {
        StartCoroutine(UpdateParameters());
        StartCoroutine(FollowTarget());
    }

    IEnumerator FollowTarget()
    {
        while (true)
        {
            if (Follow)
            {
                Pivot.rotation = Quaternion.Lerp(Pivot.rotation, Target.rotation, Time.deltaTime * RotateSmoothTime);
                Pivot.position = Vector3.Lerp(Pivot.position, Target.position, Time.deltaTime * FollowSmoothTime);
            }

            yield return null;
        }
    }

    IEnumerator UpdateParameters()
    {
        float h = -1;
        float d = -1;

        while (true)
        {
            if (Height != d || Depth != h)
            {
                //Cam.transform.localPosition.Set(0, Height, Depth);
                Cam.transform.localPosition = new Vector3(0, Height, Depth);
                h = Height;
                d = Depth;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
