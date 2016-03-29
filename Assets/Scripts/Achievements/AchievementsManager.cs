using System.Collections;
using System.Linq;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public RaceManager RaceManager;
    public NewAchievementUI ShowAchievementUI;
    [Range(1, 60)]
    public int RefreshRate; // Refresh Rate, in Times Per Second

    void Start()
    {
        RaceManager = RaceManager.Instance;;

        StartCoroutine(GetCarCoroutine());


    }

    IEnumerator VerifyAchievements(Car car)
    {
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;
        var achievementVerified = new bool[AchievementsInfo.GetTotalAchievementsCount()];

        for (var i = 0; i < achievementVerified.Length; i++)
            achievementVerified[i] = AchievementsIO.GetAchievementStatus(i) == 1;

        while (car != null)
        {
            for (var i = 0; i < achievementVerified.Length; i++)
            {
                if (!achievementVerified[i] && VerifyAchievementWithId(car, i))
                {
                    achievementVerified[i] = true;
                    ShowAchievementUI.ShowAchievement(AchievementsInfo.GetAchievementDescription(i));
                    AchievementsIO.ChangeAchievementStatus(i);
                }
            }

            if (oldRefreshRate != RefreshRate)
            {
                oldRefreshRate = RefreshRate;
                refreshRateSec = 1f / RefreshRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }
    }

    IEnumerator GetCarCoroutine()
    {
        var oldRefreshRate = int.MaxValue;
        var refreshRateSec = 1f;
        Car car = null;

        while (car == null)
        {

            if (RaceManager.CarsManager != null && RaceManager.CarsManager.Cars != null && RaceManager.CarsManager.Cars.Count > 0)
                car = RaceManager.CarsManager.Cars.First();

            if (oldRefreshRate != RefreshRate)
            {
                oldRefreshRate = RefreshRate;
                refreshRateSec = 1f / RefreshRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }

        StartCoroutine(VerifyAchievements(car));
    }

    private bool VerifyAchievementWithId(Car car, int id)
    {
        switch (id)
        {
            case 0:
                var currentVelocity = car.CarMovement.Rigidbody.velocity.magnitude;
                var topSpeed = car.CarMovement.SpeedStatsKMH.CurrentTopSpeed;

                return (UnitConverter.VelocityToKMH(currentVelocity) >= topSpeed);

            case 1:
                return (car.Coins >= 50);

            case 2:
                return (car.CarMovement.WallHitCount > 0);
        }

        return false;
    }
}
