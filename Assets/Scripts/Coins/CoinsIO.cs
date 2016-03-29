using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;

public static class CoinsIO
{
    public static void Initialize(StreamWriter writer)
    {
        writer.WriteLine(GetOpeningParentNode());
        writer.WriteLine(GetChildNode());
        writer.WriteLine(GetClosingParentNode());
    }

    private static string GetOpeningParentNode()
    {
        return "    <Coins>";
    }

    private static string GetClosingParentNode()
    {
        return "    </Coins>";
    }

    private static string GetChildNode()
    {
        return "        <CoinCount value='0'/>";
    }

    private static XmlNode GetCoinsNode(XmlDocument xmlDoc)
    {
        return xmlDoc.SelectSingleNode("/Document/Coins").ChildNodes[0];
    }

    public static void SetCoinCount(int value)
    {
        var xmlDoc = CoinsFile.Open();
        var node = GetCoinsNode(xmlDoc);

        node.Attributes[0].Value = string.Concat(value);

        xmlDoc.Save(CoinsFile.Filename);
    }

    public static int GetCoinCount()
    {
        var xmlDoc = CoinsFile.Open();
        var node = GetCoinsNode(xmlDoc);
        return int.Parse(node.Attributes[0].Value);
    }
}
