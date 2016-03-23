using UnityEngine;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    public Transform PillarA, PillarB;
    public BoxCollider Trigger;
    public Vector3 Delta, Scale;
    public bool Visible;
    public bool Starting;

    void Start()
    {
        var delta = (PillarB.position - PillarA.position);
        Trigger.size = new Vector3(delta.magnitude, Scale.y, Scale.z);
        Trigger.transform.position = Vector3.Lerp(PillarA.position, PillarB.position, 0.5f) + Delta;
        Trigger.transform.rotation = Quaternion.LookRotation(Vector3.Cross(delta, Vector3.up), Vector3.up);
    }
}

