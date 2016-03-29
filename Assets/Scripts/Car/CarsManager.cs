using UnityEngine;
using System.Collections.Generic;
using System;

public class CarsManager : MonoBehaviour
{
    public List<Car> Cars;

    public void ResetAllCars()
    {
        foreach (var car in Cars)
            car.Reset();
    }

    public void AssignStartingPositions(List<Transform> startingPositions, bool random)
    {
        var positions = new List<Transform>(startingPositions);
        if (random)
            positions = Utils.ShuffleList(positions);

        for (int i = 0; i < Cars.Count; i++)
        {
            Cars[i].transform.position = positions[i].position;
            Cars[i].transform.rotation = positions[i].rotation;
        }
    }

    public void ReleaseAllCars()
    {
        foreach (var car in Cars)
            car.CarMovement.State.CanMove = true;
    }
}
