using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public List<Checkpoint> Checkpoints;

    void Start()
    {
        //Checkpoints = new List<Checkpoint>(FindObjectsOfType<Checkpoint>());
    }

    public int GetStartingCheckpoint()
    {
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            if (Checkpoints[i].Starting)
                return i;
        }

        Debug.LogWarning("Did not find starting checkpoint. Will return -1.");
        return -1;
    }

    public int GetNextCheckpoint(int currentCheckpoint)
    {
        return (currentCheckpoint + 1) % Checkpoints.Count;
    }

    public int GetCheckpointIndex(Collider col)
    {
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            if (Checkpoints[i].Trigger == col)
                return i;
        }

        return -1;
    }
}
