using UnityEngine;

public class LapCounter : MonoBehaviour
{
    [SerializeField]
    RaceManager _raceManager;

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
        _raceManager = RaceManager.Instance;

        PassedCheckpoints = 0;
        NextCheckpoint = _raceManager.GetFirstCheckpoint();
    }
    
    void LateUpdate()
    {
        CurrentLap = _raceManager.GetCurrentLap(PassedCheckpoints);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_raceManager.IsColliderOfCheckpoint(other, NextCheckpoint))
        {
            NextCheckpoint = _raceManager.GetNextCheckpoint(NextCheckpoint);
            PassedCheckpoints++;
        }
    }
}
