using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(LapCounter))]
[RequireComponent(typeof(LapTimeCounter))]
public class Car : MonoBehaviour
{
    public string PlayerName;
    public int PlayerIndex, CarId, Coins;
    public int PlaceInRace, RankingsPlace;
    public AudioSource CoinCollect;
    [Range(1, 60)]
    public int CheckFinishRate;

    public ICollection<Upgrade> Upgrades;

    CarMovement _carMovement;
    LapCounter _lapCounter;
    LapTimeCounter _timeCounter;
    RaceManager _raceManager;

    public RaceManager RaceManager
    {
        get
        {
            if (_raceManager == null)
                _raceManager = RaceManager.Instance;

            return _raceManager;
        }
    }

    public CarMovement CarMovement
    {
        get
        {
            if (_carMovement == null)
                _carMovement = GetComponent<CarMovement>();
            return _carMovement;
        }
    }

    public LapCounter LapCounter
    {
        get
        {
            if (_lapCounter == null)
                _lapCounter = GetComponent<LapCounter>();
            return _lapCounter;
        }
    }

    public LapTimeCounter LapTimeCounter
    {
        get
        {
            if (_timeCounter == null)
                _timeCounter = GetComponent<LapTimeCounter>();
            return _timeCounter;
        }
    }

    public bool Finished
    {
        get
        {
            return LapCounter.CurrentLap >= RaceManager.NumLaps;
        }
    }

    void Start()
    {
        Upgrades = new List<Upgrade>();
        Coins = 0;

        StartCoroutine(CheckForFinish());
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Finished && other.gameObject.CompareTag("Coin"))
        {
            Coins++;
            CoinCollect.Play();

            StartCoroutine(DeactivateCoint(other));
        }
    }

    IEnumerator CheckForFinish()
    {
        var oldFinished = Finished;
        var oldCheckRate = int.MaxValue;
        var refreshRateSec = 1f;

        while (true)
        {
            if (Finished != oldFinished)
            {
                oldFinished = Finished;
                if (Finished)
                {
                    CarMovement.State.CanMove = !Finished;
                    CarMovement.Rigidbody.velocity = Vector3.zero;
                    CarMovement.Rigidbody.angularVelocity = Vector3.zero;
                }
            }

            if (oldCheckRate != CheckFinishRate)
            {
                oldCheckRate = CheckFinishRate;
                refreshRateSec = 1f / CheckFinishRate;
            }

            yield return new WaitForSeconds(refreshRateSec);
        }
    }

    IEnumerator DeactivateCoint(Collider coin)
    {
        coin.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        coin.gameObject.SetActive(true);
    }

    public void Reset()
    {
        PlaceInRace = 0;
        RankingsPlace = 0;

        CarMovement.Reset();
        LapCounter.Reset();
        LapTimeCounter.Reset();
    }
}