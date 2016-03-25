using System.IO;
using System.Collections.Generic;

public static class RaceWriter
{
    public static void WriteKeyValue(string filename, string key, string value)
    {
        using (var writer = new StreamWriter(filename, true))
        {
            var kv = new KeyValue(key, value);
            writer.WriteLine(kv.ToString());
            writer.Close();
        }
    }

    public static void UpdateKeyValue(string filename, string key, string value)
    {
        if (!File.Exists(filename))
        {
            WriteKeyValue(filename, key, value);
            return;
        }

        bool found = false;

        var lines = File.ReadAllLines(filename);
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var kv = KeyValue.Parse(line);

            if (kv.Key == key)
            {
                found = true;
                kv.Value = value;
                lines[i] = kv.ToString();
            }
        }

        if (found)
            File.WriteAllLines(filename, lines);
        else
            WriteKeyValue(filename, key, value);
    }
}

public static class RaceReader
{
    public static string Filename = "RaceValues.sav";

    public static List<KeyValue> ReadAllValues(string filename)
    {
        List<KeyValue> list = new List<KeyValue>();

        if (File.Exists(filename))
        {
            using (var reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    var kv = KeyValue.Parse(reader.ReadLine());
                    if (kv != null)
                        list.Add(kv);
                }


                reader.Close();
            }
        }

        return list;
    }
}

public class KeyValue
{
    public static char Separator = ' ';
    public string Key, Value;

    public KeyValue() { }

    public KeyValue(string nKey, string nValue)
    {
        Key = nKey;
        Value = nValue;
    }

    public static KeyValue Parse(string line)
    {
        var args = line.Split(Separator);
        if (args.Length < 2)
            return null;

        var kv = new KeyValue();

        kv.Key = args[0];
        kv.Value = args[1];

        return kv;
    }

    public override string ToString()
    {
        return string.Concat(Key, Separator, Value);
    }
}