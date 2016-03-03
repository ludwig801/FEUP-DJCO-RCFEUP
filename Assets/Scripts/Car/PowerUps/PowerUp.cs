using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class PowerUp : MonoBehaviour
{
    public enum Types
    {
        SPEED_UP = 0,
        BOOST = 1
    }

    public Types Type;
    public bool Accumulable;
    public bool CanBeTaken;
    public GameObject Target;

    public abstract void Apply();
}
