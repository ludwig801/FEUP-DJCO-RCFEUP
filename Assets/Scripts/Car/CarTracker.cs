using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarTracker : MonoBehaviour
{
    public int PassedCheckpoints;

    [SerializeField]
    int _currentLap;
    [SerializeField]
    int _currentCheckpoint;
    [SerializeField]
    TrackManager _trackManager;

    void Start()
    {
        _trackManager = TrackManager.Instance;

        PassedCheckpoints = 0;
        _currentCheckpoint = _trackManager.GetFirstCheckpoint();
    }
    
    void LateUpdate()
    {
        _currentLap = _trackManager.GetCurrentLap(PassedCheckpoints);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_trackManager.IsColliderOfCheckpoint(other, _currentCheckpoint))
        {
            _currentCheckpoint = _trackManager.GetNextCheckpoint(_currentCheckpoint);
            PassedCheckpoints++;
        }
    }
}
