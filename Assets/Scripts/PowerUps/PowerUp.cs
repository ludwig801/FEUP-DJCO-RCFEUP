using UnityEngine;
using System.Collections;

public abstract class PowerUp : MonoBehaviour
{
    public enum Types
    {
        TorqueBoost = 0,
        TopSpeedBoost = 1
    }

    public PowerUpManager PowerUps;
    public FloatAnimation FloatAnimation;
    public RotateAnimation RotateAnimation;
    public Types Type;
    public string Description;
    public Color AccentColor;
    public Car Target;
    public float Duration;
    public bool Available;

    Coroutine _lastApply;
    MeshRenderer _renderer;

    MeshRenderer Renderer
    {
        get
        {
            if(_renderer == null)
                _renderer = GetComponentInChildren<MeshRenderer>();

            return _renderer;
        }
    }

    protected virtual void Start()
    {
        PowerUps = RaceManager.Instance.PowerUps;

        Renderer.materials[1].color = AccentColor;
        StartCoroutine(FloatAnimation.Run());
        StartCoroutine(RotateAnimation.Run());
    }

    protected virtual void Stop()
    {
        if (_lastApply != null)
            StopCoroutine(_lastApply);

        Reset();
    }

    protected abstract bool ApplyEffects();

    protected abstract void RemoveEffects();

    private IEnumerator Apply(Car car)
    {
        Target = car;
        Available = false;
        Renderer.enabled = false;

        var powerUp = PowerUps.GetTargetPowerUp(Target, this);
        if (powerUp != null)
            powerUp.Stop();

        if (ApplyEffects())
        {
            PowerUps.PlaySound();

            yield return new WaitForSeconds(Duration);
        }

        Reset();
    }

    public void Reset()
    {
        if(Target != null)
            RemoveEffects();

        Renderer.enabled = true;
        Target = null;
        Available = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!Available)
            return;

        var car = other.GetComponentInParent<Car>();
        if (car == null)
            return;

        if (_lastApply != null)
            StopCoroutine(_lastApply);
        _lastApply = StartCoroutine(Apply(car));
    }
}

[System.Serializable]
public class Animation
{
    public bool Active;
    public Transform Target;
    public float Duration;
}

[System.Serializable]
public class FloatAnimation : Animation
{
    public float DeltaY;

    public IEnumerator Run()
    {
        if (Target == null)
        {
            Debug.LogWarning(string.Concat("Target for this powerup's floating animation is null. Animation will not run."));
            yield break;
        }

        var oldFloatingDelta = DeltaY;
        var initialPos = Target.position;
        var topPosition = initialPos + new Vector3(0, DeltaY, 0);
        var oldDuration = Duration;
        var invertedAnimDuration = 1f / Duration;
        var timeDelta = 0f;
        var floatingUp = true;

        while (true)
        {
            if (Active)
            {
                if (oldFloatingDelta != DeltaY)
                {
                    topPosition = initialPos + new Vector3(0, DeltaY, 0);
                }

                if (oldDuration != Duration && Duration != 0)
                {
                    oldDuration = Duration;
                    invertedAnimDuration = 1f / Duration;
                }

                timeDelta += Time.deltaTime;
                var timePercentage = timeDelta * invertedAnimDuration;
                Target.position = Vector3.Lerp(floatingUp ? initialPos : topPosition, floatingUp ? topPosition : initialPos, Mathf.SmoothStep(0, 1, timePercentage));

                if (timePercentage >= 1)
                {
                    timeDelta = 0;
                    floatingUp = !floatingUp;
                }

                yield return null;
            }
            else yield return new WaitForSeconds(1);
        }
    }
}

[System.Serializable]
public class RotateAnimation : Animation
{
    public enum Axis
    {
        X, Y, Z
    }

    public bool Clockwise;
    public Axis RotationAxis;

    public IEnumerator Run()
    {
        if (Target == null)
        {
            Debug.LogWarning(string.Concat("Target for this powerup's rotating animation is null. Animation will not run."));
            yield break;
        }

        var oldDuration = Duration;
        var rotationSpeed = 360 / Duration;
        var oldClockwise = Clockwise;
        var rotationMult = Clockwise ? 1 : -1;
        var rotationAxis = RotationAxis == Axis.X ? Vector3.right : RotationAxis == Axis.Y ? Vector3.up : Vector3.forward;

        while (true)
        {
            if (Active)
            {
                if (oldClockwise != Clockwise)
                {
                    oldClockwise = Clockwise;
                    rotationMult = Clockwise ? 1 : -1;
                }

                if (oldDuration != Duration && Duration != 0)
                {
                    oldDuration = Duration;
                    rotationSpeed = 360 / Duration;
                }

                Target.Rotate(rotationAxis, Time.deltaTime * rotationSpeed * rotationMult);

                yield return null;
            }
            else yield return new WaitForSeconds(1);
        }
    }
}