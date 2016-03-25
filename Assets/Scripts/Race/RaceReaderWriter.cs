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
            return;

        var lines = File.ReadAllLines(filename);
        foreach (var line in lines)
        {
            var kv = KeyValue.Parse(line);

            if (kv.Key == key)
                kv.Value = value;
        }

        File.WriteAllLines(filename, lines);
    }
}

public static class RaceReader
{
    public static List<KeyValue> ReadAllValues(string filename, string key, string value)
    {
        List<KeyValue> list = new List<KeyValue>();

        using (var reader = new StreamReader(filename, true))
        {
            while(!reader.EndOfStream)
                list.Add(KeyValue.Parse(reader.ReadLine()));

            reader.Close();
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