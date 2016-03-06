using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    public int UpgradeId;

    public int Level;

    public int Increment;

    public double PriceForNextLevel;

    public abstract void Apply(CarMovement car);
    public abstract void Remove();
}