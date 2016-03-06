using System.IO;

public static class UpgradeWriter
{
    public static void Save(Upgrade upgrade)
    {
        using (var streamWriter = new StreamWriter("upgrades" + ".sav"))
        {
            streamWriter.WriteLine("<Upgrades>");

            var upgradeLine = "<Upgrade id='" + upgrade.UpgradeId + "'/>";
            streamWriter.WriteLine(upgradeLine);

            streamWriter.WriteLine("</Upgrades>");

            streamWriter.Close();
        }

    }
}

