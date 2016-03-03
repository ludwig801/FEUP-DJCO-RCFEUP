using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class PowerUp : MonoBehaviour
{
    public enum Types
    {
        BOOST = 0
    }

    public Types Type;
    public bool Accumulable;
    public bool CanBeTaken;
    public GameObject Target;

    public abstract void Apply();
}
