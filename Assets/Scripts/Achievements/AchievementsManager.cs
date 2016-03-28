using System.Collections;
using System.Linq;
using UnityEngine;

public class AchievementsManager : MonoBehaviour {

    [Range(1, 60)]
    public float RefreshRate; // Refresh Rate, in Times Per Second

    private bool[] _achievementsStatus = new bool[AchievementsInfo.GetTotalAchievementsCount()];

    public RaceManager RaceManager;

    private Car _car;

	void Start () {

        RefreshRate = 60;

        _car = null;

        StartCoroutine(GetCarCoroutine());
	    
        for(var i = 0; i < _achievementsStatus.Length; i++)
        {
            _achievementsStatus[i] = AchievementsIO.GetAchievementStatus(i) == 1;
        }

	}

	void Update () {

        if(_car != null)
        {
            for (var i = 0; i < _achievementsStatus.Length; i++)
            {
                if (!_achievementsStatus[i])
                {
                    if (VerifyAchievementWithId(i))
                    {
                        _achievementsStatus[i] = true;
                        AchievementsIO.ChangeAchievementStatus(i);
                    }
                }
            }
        }

	}

    IEnumerator GetCarCoroutine()
    {
        float oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;

        while(_car == null)
        {

            if(RaceManager.CarsManager != null && RaceManager.CarsManager.Cars != null && RaceManager.CarsManager.Cars.Count > 0)
            {
                _car = RaceManager.CarsManager.Cars.First();
            }

            if(oldRefreshRate != RefreshRate)
            {
                oldRefreshRate = RefreshRate;
                refreshRateSec = 1f / RefreshRate;
            }

            yield return new WaitForSeconds(refreshRateSec);

        }
    }

    private bool VerifyAchievementWithId(int id)
    {
        switch(id)
        {
            case 0:
                var currentVelocity = _car.CarMovement.Rigidbody.velocity.magnitude;
                var topSpeed = _car.CarMovement.SpeedStatsKMH.CurrentTopSpeed;

                return (UnitConverter.VelocityToKMH(currentVelocity) >= topSpeed);

            case 1:
                return (_car.Coins > 0);
                
            case 2:
                return (_car.CarMovement.WallHitCount > 0);
        }

        return false;
    }
}
