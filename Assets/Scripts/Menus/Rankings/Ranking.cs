using System;

[System.Serializable]
public class Ranking : IComparable<Ranking>
{
    public int Place;
    public string PlayerName;
    public float PlayerTime;

    int IComparable<Ranking>.CompareTo(Ranking other)
    {
        if (Place > other.Place)
            return 1;
        else if (Place < other.Place)
            return -1;

        return 0;
    }

    public override string ToString()
    {
        return string.Concat(Place, " | ", PlayerName, " | ", PlayerTime);
    }
}
