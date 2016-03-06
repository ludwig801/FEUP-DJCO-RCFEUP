using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    public int UpgradeId;

    public string Name;

    public string Description;

    public double PriceInEuros;

    public abstract void Apply(CarMovement car);
    public abstract void Remove();
}