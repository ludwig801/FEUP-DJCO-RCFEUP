using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public List<Checkpoint> Checkpoints;

    void Start()
    {
        //Checkpoints = new List<Checkpoint>(FindObjectsOfType<Checkpoint>());
    }
}
