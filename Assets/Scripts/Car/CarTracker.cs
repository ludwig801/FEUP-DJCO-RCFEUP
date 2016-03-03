using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarTracker : MonoBehaviour
{
    [SerializeField]
    TrackManager _trackManager;

    public int CurrentLap
    {
        get;
        private set;
    }

    public int PassedCheckpoints
    {
        get;
        private set;
    }

    public int NextCheckpoint
    {
        get;
        private set;
    }

    void Start()
    {
        _trackManager = TrackManager.Instance;

        PassedCheckpoints = 0;
        NextCheckpoint = _trackManager.GetFirstCheckpoint();
    }
    
    void LateUpdate()
    {
        CurrentLap = _trackManager.GetCurrentLap(PassedCheckpoints);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_trackManager.IsColliderOfCheckpoint(other, NextCheckpoint))
        {
            NextCheckpoint = _trackManager.GetNextCheckpoint(NextCheckpoint);
            PassedCheckpoints++;
        }
    }
}
