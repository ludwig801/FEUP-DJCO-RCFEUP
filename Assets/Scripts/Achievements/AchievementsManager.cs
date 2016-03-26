using System;
using UnityEngine;

public class AchievementsManager : MonoBehaviour {

    private bool[] _achievementsStatus = new bool[AchievementsInfo.GetTotalAchievementsCount()];

    public Car Car;

	void Start () {

        Car = GetComponent<Car>();
	    
        for(var i = 0; i < _achievementsStatus.Length; i++)
        {
            _achievementsStatus[i] = AchievementsIO.GetAchievementStatus(i) == 1;
        }

	}

	void Update () {

        for(var i = 0; i < _achievementsStatus.Length; i++)
        {
            if(!_achievementsStatus[i])
            {
                if(VerifyAchievementWithId(i))
                {
                    AchievementsIO.ChangeAchievementStatus(i);
                }
            }
        }

	}

    private bool VerifyAchievementWithId(int id)
    {
        switch(id)
        {
            case 0:
                var currentVelocity = Car.CarMovement.Rigidbody.velocity.magnitude;
                var topSpeed = Car.CarMovement.SpeedStatsKMH.CurrentTopSpeed;

                Debug.Log("Velocity: " + currentVelocity);
                Debug.Log("Top Speed: " + topSpeed);

                if(UnitConverter.VelocityToKMH(currentVelocity) >= topSpeed)
                {
                    return true;
                }

                break;
            case 1:
                break;
            case 2:
                break;
        }

        return false;
    }
}
