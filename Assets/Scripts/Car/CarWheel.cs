using UnityEngine;

public class CarWheel : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public LayerMask LayerMask;
    public float SpringLength, TireRadius;
    [Range(0, 2)]
    public float Spring;
    public bool ApplyDamping;
    [Range(0, 2)]
    public float Damping;

    private float _lastCompression;
    private Vector3 _lastNormal;
    private Vector3 _lastHitPoint;
    private bool _grounded;

    public float Compression
    {
        get
        {
            return _lastCompression;
        }
    }

    public Vector3 Normal
    {
        get
        {
            return _lastNormal;
        }
    }

    public Vector3 HitPoint
    {
        get
        {
            return _lastHitPoint;
        }
    }

    public bool Grounded
    {
        get
        {
            return _grounded;
        }
    }

    private void Start()
    {
        _lastCompression = 0;
        _grounded = false;
    }

    private void FixedUpdate()
    {
        var position = transform.position;
        var ray = new Ray(position, -Rigidbody.transform.up);
        var hitInfo = new RaycastHit();
        var rayLength = SpringLength + TireRadius;
        _grounded = false;

        if (Physics.Raycast(ray, out hitInfo, rayLength, LayerMask))
        {
            var compression = 0.5f - ((hitInfo.point - position).magnitude - TireRadius) / SpringLength;
            Debug.DrawRay(position, hitInfo.point - position, Color.Lerp(Color.green, Color.red, compression));

            var springForce = Spring * compression;
            var springVector = Rigidbody.transform.up * springForce;
            Rigidbody.AddForceAtPosition(springVector * Rigidbody.mass, hitInfo.point, ForceMode.Impulse);
            Debug.DrawRay(position, springVector, Color.blue);

            if (ApplyDamping)
            {
                var dampingForce = Damping * (compression - _lastCompression);
                var dampingVector = Rigidbody.transform.up * dampingForce;
                Rigidbody.AddForceAtPosition(dampingVector * Rigidbody.mass, hitInfo.point, ForceMode.Impulse);
                Debug.DrawRay(position, dampingVector, Color.red);
            }

            _lastCompression = compression;
            _lastHitPoint = hitInfo.point;
            _lastNormal = hitInfo.normal;
            _grounded = true;
        }
    }
}