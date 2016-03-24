using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(LapCounter))]
[RequireComponent(typeof(LapTimeCounter))]
public class Car : MonoBehaviour
{
    public int CarId;
    public int Coins;
    public AudioSource CoinCollect;

    public ICollection<Upgrade> Upgrades;

    CarMovement _carMovement;
    LapCounter _lapCounter;
    LapTimeCounter _timeCounter;

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

    void Start()
    {
        Upgrades = new List<Upgrade>();
        Coins = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Coins++;
            CoinCollect.Play();

            StartCoroutine(DeactivateCoint(other));
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
        CarMovement.Reset();
        LapCounter.Reset();
        LapTimeCounter.Reset();
    }
}