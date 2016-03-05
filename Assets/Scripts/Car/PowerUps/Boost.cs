using UnityEngine;
using System.Collections;

public class Boost : PowerUp
{
    public float Percentage;
    public float Duration;
    public float TimeLeft;

    void Start()
    {
        CanBeTaken = true;
    }

    public override void Apply()
    {
        var carMovement = Target.GetComponentInParent<CarMovement>();
        if (carMovement == null)
        {
            Debug.LogWarning("Car movement is null! Power-up [Boost] cannot be applied.");
            return;
        }

        StartCoroutine(Execute(carMovement));
    }

    IEnumerator Execute(CarMovement carMovement)
    {
        if (carMovement.PowerUp != null && carMovement.PowerUp.Type == Type)
        {
            CanBeTaken = true;

            if (Accumulable)
                ((Boost)carMovement.PowerUp).TimeLeft += Duration;

            yield break;
        }

        carMovement.PowerUp = this;
        TimeLeft = Duration;
        var oldValue = carMovement.CurrentTopSpeedKMH;
        carMovement.CurrentTopSpeedKMH = Mathf.Max(carMovement.CurrentTopSpeedKMH, carMovement.CurrentTopSpeedKMH * Percentage);

        while (TimeLeft >= 0)
        {
            TimeLeft -= Time.deltaTime;
            yield return null;
        }

        carMovement.CurrentTopSpeedKMH = Mathf.Min(oldValue, carMovement.CurrentTopSpeedKMH);
        carMovement.PowerUp = null;
        CanBeTaken = true;
        yield break;
    }

    void OnTriggerEnter(Collider other)
    {
        if (CanBeTaken)
        {
            CanBeTaken = false;
            Target = other.gameObject;
            Apply();
        }
    }
}
