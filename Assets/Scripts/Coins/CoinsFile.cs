using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public static class CoinsFile
{
    public static string Filename = "Coins.sav";

    public static XmlDocument Open()
    {
        if (!File.Exists(Filename))
            Create();

        var doc = new XmlDocument();
        doc.Load(Filename);

        return doc;
    }

    private static void Create()
    {
        Debug.Log("File does not exist. Creating...");
        using (var streamWriter = new StreamWriter(Filename, true))
        {
            streamWriter.WriteLine(GetXmlVersion());
            streamWriter.WriteLine(GetOpeningDocumentTag());

            CoinsIO.Initialize(streamWriter);

            streamWriter.WriteLine(GetClosingDocumentTag());
            streamWriter.Close();
        }    
    }

    private static string GetXmlVersion()
    {
        return "<?xml version='1.0' encoding='utf-8'?>";
    }

    private static string GetOpeningDocumentTag()
    {
        return "<Document>";
    }

    private static string GetClosingDocumentTag()
    {
        return "</Document>";
    }
}
