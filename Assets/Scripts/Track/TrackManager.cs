using UnityEngine;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour
{
    public static TrackManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<TrackManager>();
            return _instance;
        }
    }
    static TrackManager _instance;

    public int Start;

    [SerializeField]
    List<TrackTrigger> _checkpoints;
    [SerializeField]
    int _numLaps;

    public int NumLaps
    {
        get
        {
            return _numLaps;
        }
    }

    public int GetFirstCheckpoint()
    {
        return Start;
    }

    public int GetNextCheckpoint(int currentCheckpoint)
    {
        return (currentCheckpoint + 1) % _checkpoints.Count;
    }

    public bool IsColliderOfCheckpoint(Collider col, int checkpointIndex)
    {
        return _checkpoints[checkpointIndex].TriggerCollider == col;
    }

    public int GetCurrentLap(int passedCheckpoints)
    {
        return ((passedCheckpoints - 1) / _checkpoints.Count); // account for first checkpoint, which is always passed in the beggining
    }
}
