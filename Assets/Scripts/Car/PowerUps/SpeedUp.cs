using System.Collections;
using UnityEngine;

public class SpeedUp : PowerUp
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
            Debug.LogWarning("Car movement is null! Power-up [Speed Up] cannot be applied.");
            return;
        }

        StartCoroutine(Execute(carMovement));
    }

    IEnumerator Execute(CarMovement carMovement)
    {
        foreach (var powerup in carMovement.PowerUps)
        {
            if (powerup.Type == Type)
            {
                if (Accumulable)
                    ((SpeedUp)powerup).TimeLeft += Duration;
                else
                    CanBeTaken = true;

                yield break;
            }
        }

        carMovement.PowerUps.Add(this);
        TimeLeft = Duration;
        var oldValue = carMovement.Acceleration;
        carMovement.Acceleration = Mathf.Max(carMovement.Acceleration, carMovement.Acceleration * Percentage);

        while (TimeLeft >= 0)
        {
            TimeLeft -= Time.deltaTime;
            yield return null;
        }

        carMovement.Acceleration = Mathf.Min(oldValue, carMovement.Acceleration);
        carMovement.PowerUps.Remove(this);
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
