using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class UpgradeWriter
{
    public static void Save(Car car, ICollection<Upgrade> upgrades)
    {

        using (var streamWriter = new StreamWriter("upgrades" + ".sav"))
        {
            streamWriter.WriteLine(car.CarId);

            streamWriter.Close();
        }

    }

    private static void CreateFileIfNotExists(string filename)
    {

    }
}

