using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<RaceManager>();

            return _instance;
        }
    }
    static RaceManager _instance;

    public enum RaceTypes
    {
        TIME_TRIAL = 0
    }

    public CamerasManager CamerasManager;
    public PowerUpManager PowerUps;
    public CarsManager CarsManager;
    public CheckpointManager CheckpointManager;
    public RaceTypes RaceType;
    public int NumLaps, NumPlayers;
    public GameObject PlayerPrefab;
    [Range(1, 60)]
    public int CheckWinnerRate;
    public bool RandomCarSpot;
    public List<Transform> StartingPositions;
    public RaceState State;
    public Countdown Countdown;

    Coroutine _lastCountdown;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        ReadRaceValues();
        CreatePlayers();
        CarsManager.ResetAllCars();
        CarsManager.AssignStartingPositions(StartingPositions, true);

        State.Reset();
    }

    public void ReadRaceValues()
    {
        var kValues = RaceReader.ReadAllValues(RaceReader.Filename);

        foreach (var keyValue in kValues)
        {
            switch (keyValue.Key)
            {
                case "numPlayers":
                    NumPlayers = int.Parse(keyValue.Value);
                    break;

                case "numLaps":
                    NumLaps = int.Parse(keyValue.Value);
                    break;
            }
        }
    }

    public void CreatePlayers()
    {
        for (int i = 0; i < NumPlayers; i++)
        {
            var player = Instantiate(PlayerPrefab) as GameObject;
            player.name = "Player " + (i + 1);
            var car = player.GetComponentInChildren<Car>();
            car.PlayerIndex = i;
            var carCanvas = player.GetComponentInChildren<CarCanvas>();
            carCanvas.Car = car;
            var carMovement = car.CarMovement;
            carMovement.CarInput.Index = i;
            var chaseCam = player.GetComponentInChildren<ChaseCam>();
            chaseCam.Target = carMovement;

            ReadUpgrades(car);

            CarsManager.Cars.Add(car);
            CamerasManager.Cameras.Add(chaseCam);
        }

        CamerasManager.FitCameras();
    }

    public void ReadUpgrades(Car car)
    {
        var numLevels = 5f;
        var engine = UpgradeWriter.GetUpgradeLevel(0) / numLevels;
        var springs = UpgradeWriter.GetUpgradeLevel(1) / numLevels;
        var weightReduction = UpgradeWriter.GetUpgradeLevel(2) / numLevels;

        var grip = 1 - weightReduction;
        var accelBonus = Mathf.Min(engine * springs * grip, 0.25f);
        var speedBonus = Mathf.Min(engine + weightReduction, 0.75f);
        var handlingBonus = Mathf.Clamp(weightReduction * springs, 0.1f, 0.75f);

        var carMovement = car.CarMovement;
        carMovement.TorqueSystem.MotorTorque *= (1 + accelBonus);
        carMovement.SpeedStatsKMH.TopSpeed *= (1 + speedBonus);
        carMovement.TractionControl = handlingBonus;
    }

    public void NewRace(bool waitForAllPlayers)
    {
        State.PlayersReady++;
        if (!waitForAllPlayers || State.PlayersReady == NumPlayers)
        {
            State.Reset();
            State.Started = true;

            PowerUps.ResetAllPowerUps();
            CarsManager.ResetAllCars();
            CarsManager.AssignStartingPositions(StartingPositions, RandomCarSpot);

            if (_lastCountdown != null)
                StopCoroutine(_lastCountdown);

            _lastCountdown = StartCoroutine(CountdownAndStart());

            StartCoroutine(CheckForRaceEnd());
        }
    }

    IEnumerator CheckForRaceEnd()
    {
        var oldRefreshRate = int.MaxValue;
        var refresRateSec = 1f;

        while (true)
        {
            if (State.Ongoing)
            {
                var numCarsFinished = 0;
                foreach (var car in CarsManager.Cars)
                {
                    if (car.LapTimeCounter.LapsTimes.Count >= NumLaps)
                        numCarsFinished++;
                }
                if (numCarsFinished >= NumPlayers)
                    State.Finished = true;
            }

            if (State.Finished)
            {
                OnRaceFinished();
                yield break;
            }

            if (oldRefreshRate != CheckWinnerRate)
            {
                oldRefreshRate = CheckWinnerRate;
                refresRateSec = 1f / CheckWinnerRate;
            }

            yield return new WaitForSeconds(refresRateSec);
        }
    }

    IEnumerator CountdownAndStart()
    {
        Countdown.Running = true;
        Countdown.CurrentCount = Countdown.Duration;

        while (Countdown.CurrentCount != 0)
        {
            yield return new WaitForSeconds(1);
            Countdown.CurrentCount--;
        }

        Countdown.CurrentCount = 0;
        Countdown.Running = false;
        State.Ongoing = true;

        CarsManager.ReleaseAllCars();
    }

    public int GetCurrentLap(int passedCheckpoints)
    {
        if (passedCheckpoints == 0)
            return -1;

        // account for first checkpoint, which is always passed in the beggining
        return ((passedCheckpoints - 1) / CheckpointManager.Checkpoints.Count);
    }

    public void SetPaused(bool value)
    {
        State.Paused = value;
        Time.timeScale = value ? 0 : 1;
    }

    private void OnRaceFinished()
    {
        State.Finished = true;

        foreach (var car in CarsManager.Cars)
        {
            car.PlaceInRace = 0;
            car.RankingsPlace = 0;
        }

        for (int i = 0; i < CarsManager.Cars.Count; i++)
        {
            var car = CarsManager.Cars[i];
            var carRaceTime = car.LapTimeCounter.RaceTime;
            for (int j = 0; j < CarsManager.Cars.Count; j++)
            {
                if (j != i)
                {
                    var other = CarsManager.Cars[j];
                    if (other.LapTimeCounter.RaceTime < carRaceTime)
                        car.PlaceInRace++;
                }
            }
        }

        var rankings = RankingsReader.GetAllRankings();

        for (int i = 0; i < CarsManager.Cars.Count; i++)
        {
            var currentCar = CarsManager.Cars[i];
            var currentCarTime = currentCar.LapTimeCounter.RaceTime;
            var foundPlace = false;
            for (int j = 0; j < rankings.Count; j++)
            {
                var currentRanking = rankings[j];

                if (currentCarTime < currentRanking.PlayerTime)
                {
					if (rankings.Count == 10) { // rankings filled, only has to replace
						for (int k = rankings.Count - 1; k > j; k--) {
							rankings [k] = rankings [k - 1];
							rankings [k].Place += 1; //update the place
						}
					} else {
						rankings.Add (rankings [rankings.Count - 1]); // duplicate the last one
						for (int k = rankings.Count - 1; k > j; k--) { // replace the others
							rankings [k] = rankings [k - 1];
							rankings [k].Place += 1;
						}
					}

                    currentCar.RankingsPlace = j + 1;
                    var r = new Ranking();
                    r.Place = currentCar.RankingsPlace;
                    r.PlayerName = currentCar.PlayerName;
                    r.PlayerTime = currentCarTime;

                    rankings[j] = r;
                    foundPlace = true;
                    break;
                }
            }

            if (!foundPlace)
            {
                currentCar.RankingsPlace = rankings.Count + 1;
                var r = new Ranking();
                r.Place = currentCar.RankingsPlace;
                r.PlayerName = currentCar.PlayerName;
                r.PlayerTime = currentCarTime;

                rankings.Add(r);
            }
        }

        RankingsWriter.WriteToFile(rankings);
    }

    public void OnQuitRace(bool waitForAllPlayers = false)
    {
        State.QuitPlayers++;
        if (!waitForAllPlayers || State.QuitPlayers == NumPlayers)
        {
            SaveCoins();
            State.QuitPlayers = 0;
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void SaveCoins()
    {
        var coinsSum = 0;
        foreach (var car in CarsManager.Cars)
            coinsSum += car.Coins;

        var oldCoins = CoinsIO.GetCoinCount();

        coinsSum += oldCoins;

        CoinsIO.SetCoinCount(coinsSum);
    }
}

[System.Serializable]
public class RaceState
{
    public bool Started, Ongoing, Paused, Finished;
    public int PlayersReady, QuitPlayers;

    internal void Reset()
    {
        QuitPlayers = 0;
        PlayersReady = 0;
        Started = false;
        Paused = false;
        Ongoing = false;
        Finished = false;
    }
}

[System.Serializable]
public class Countdown
{
    [Range(1, 5)]
    public int Duration;
    public int CurrentCount;
    public bool Running;

    internal void Reset()
    {
        Running = false;
    }
}