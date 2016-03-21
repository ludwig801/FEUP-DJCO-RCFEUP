using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(LapCounter))]
[RequireComponent(typeof(LapTimeCounter))]
public class Car : MonoBehaviour
{
	public int CarId;

    public ICollection<Upgrade> Upgrades;

    public Text coinsText;

    CarMovement _carMovement;
    LapCounter _lapCounter;
    LapTimeCounter _timeCounter;

    private int _coinsCount;

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
        _coinsCount = 0;
        SetCoinsText();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Coin"))
        {
            _coinsCount++;
            Destroy(other.gameObject);
            SetCoinsText();
        }
    }

    private void SetCoinsText()
    {
        coinsText.text = "Coins: " + _coinsCount;
    }
}