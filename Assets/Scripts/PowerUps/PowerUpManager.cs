using UnityEngine;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    public List<PowerUp> PowerUps;
    public AudioSource PowerUpAudio;

    void Start()
    {
        PowerUps = new List<PowerUp>(FindObjectsOfType<PowerUp>());
    }

    public PowerUp GetTargetPowerUp(Car car, PowerUp ignore = null)
    {
        foreach (var powerUp in PowerUps)
        {
            if(powerUp != ignore && powerUp.Target == car)
                return powerUp;
        }

        return null;
    }

    public void ResetAllPowerUps()
    {
        foreach (var powerUp in PowerUps)
        {
            powerUp.Reset();
        }
    }

    public void PlaySound()
    {
        PowerUpAudio.Play();
    }
}
