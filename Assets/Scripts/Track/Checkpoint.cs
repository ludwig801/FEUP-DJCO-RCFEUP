using UnityEngine;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    public Transform PillarA, PillarB, Board;
    public BoxCollider Trigger;
    public Vector3 Delta, Scale;
    public bool Visible;
    public bool Starting;
    public float BoardRotation, BoardHeight;

    void Start()
    {
        var delta = (PillarB.position - PillarA.position);
        Trigger.size = new Vector3(delta.magnitude, Scale.y, Scale.z);
        Trigger.transform.position = Vector3.Lerp(PillarA.position, PillarB.position, 0.5f) + Delta;
        Trigger.transform.rotation = Quaternion.LookRotation(Vector3.Cross(delta, Vector3.up), Vector3.up);
        Board.localScale = new Vector3(0.5f * delta.magnitude, BoardHeight, 1);
        Board.position = Vector3.Lerp(PillarA.position, PillarB.position, 0.5f) + new Vector3(0, Trigger.size.y, 0);
        Board.rotation = Trigger.transform.rotation;
        Board.localRotation = Quaternion.Euler(BoardRotation, 0, 0);
    }
}

