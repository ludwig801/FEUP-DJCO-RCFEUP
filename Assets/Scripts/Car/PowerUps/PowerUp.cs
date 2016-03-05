using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public abstract class PowerUp : MonoBehaviour
{
    public enum Types
    {
        BOOST = 0
    }

    public Types Type;
    public bool Accumulative;
    public bool CanBeTaken;
    public GameObject Target;
    public float FloatingSpeed;
    public float FloatingDelta;
    public float RotationSpeed;

    [SerializeField]
    Transform _meshTransform;
    [SerializeField]
    ParticleSystem _particlesOnCatch;
    Vector3 _initialPos, _overPos;
    bool _goingUp;
    Coroutine _lastShowCatch;

    public virtual void Start()
    {
        _meshTransform = GetComponentInChildren<MeshRenderer>().transform;
        _goingUp = true;

        _particlesOnCatch = GetComponentInChildren<ParticleSystem>();

        _initialPos = _meshTransform.position;
        StartCoroutine(FloatMesh());
    }

    public virtual void Update()
    {
        _meshTransform.gameObject.SetActive(CanBeTaken);
    }

    IEnumerator FloatMesh()
    {
        var t = 0f;
        while (true)
        {
            _overPos = _initialPos + new Vector3(0, FloatingDelta, 0);

            t += Time.deltaTime * FloatingSpeed;
            _meshTransform.position = Vector3.Lerp(_goingUp ? _initialPos : _overPos, _goingUp ? _overPos : _initialPos, InOutQuadBlend(t));

            if (Vector3.Distance(_meshTransform.position, _goingUp ? _overPos : _initialPos) < 0.01f)
            {
                t = 0f;
                _goingUp = !_goingUp;
            }

            yield return null;
        }
    }

    float InOutQuadBlend(float t)
    {
        if (t <= 0.5f)
            return 2 * t * t;

        t -= 0.5f;

        return 2 * t * (1 - t) + 0.5f;
    }

    public abstract void Apply();

    IEnumerator ShowCatch()
    {
        _particlesOnCatch.Play();

        float t = 0;
        while (t <= _particlesOnCatch.duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        _particlesOnCatch.Stop();
        _particlesOnCatch.Clear();
        _particlesOnCatch.time = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (CanBeTaken)
        {
            if (_lastShowCatch != null)
                StopCoroutine(_lastShowCatch);
            _lastShowCatch = StartCoroutine(ShowCatch());

            CanBeTaken = false;
            Target = other.gameObject;
            Apply();
        }
    }
}
