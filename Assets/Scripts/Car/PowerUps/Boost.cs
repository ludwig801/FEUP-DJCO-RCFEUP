using UnityEngine;
using System.Collections;

public class Boost : PowerUp
{
    public float Factor;

    Coroutine _applied;

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

        Stop();
        _applied = StartCoroutine(Execute(carMovement));
    }

    public override void Stop()
    {
        if (_applied != null)
            StopCoroutine(_applied);

        CanBeTaken = true;
    }

    IEnumerator Execute(CarMovement carMovement)
    {
        //CanBeTaken = false;
        //carMovement.PowerUp = this;
        //TimeLeft = Duration;
        //carMovement.CurrentTopSpeedKMH = carMovement.TopSpeedKMH * Factor;

        //while (TimeLeft > 0)
        //{
        //    TimeLeft -= Time.deltaTime;
        //    yield return null;

        //    if (carMovement.PowerUp != this)
        //    {
        //        Stop();
        //        yield break;
        //    }
        //}

        //carMovement.CurrentTopSpeedKMH = carMovement.TopSpeedKMH;
        //carMovement.PowerUp = null;
        //CanBeTaken = true;
        yield break;
    }
}
