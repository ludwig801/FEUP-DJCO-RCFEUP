using UnityEngine;
using System.Collections;

public class Boost : PowerUp
{
    public float Factor;
    public float Duration;
    public float TimeLeft;

    public override void Start()
    {
        base.Start();
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
            if (Accumulative)
                ((Boost)carMovement.PowerUp).TimeLeft += Duration;

            yield return new WaitForSeconds(Duration);
            CanBeTaken = true;
        }

        carMovement.PowerUp = this;
        TimeLeft = Duration;
        var oldValue = carMovement.TopSpeedKMH;
        carMovement.TopSpeedKMH = Mathf.Max(carMovement.TopSpeedKMH, carMovement.TopSpeedKMH * Factor);

        yield return new WaitForSeconds(Duration);

        carMovement.TopSpeedKMH = Mathf.Min(oldValue, carMovement.TopSpeedKMH);
        carMovement.PowerUp = null;
        CanBeTaken = true;
        yield break;
    }
}
