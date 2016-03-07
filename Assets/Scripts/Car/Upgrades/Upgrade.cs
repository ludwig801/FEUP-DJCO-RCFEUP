public class Upgrade
{
    private const int MaxLevel = 5;

    public int UpgradeId;

    public int Level;

    public int Increment;

    public double PriceForNextLevel;

    public bool CanIncrementLevel
    {
        get
        {
            return Level < MaxLevel;
        }
    }

    public int GetIncrementByLevel()
    {
        switch(UpgradeId)
        {
            case 0:
                return Level * 10;
            case 1:
                return Level * 10;
            case 2:
                return (Level * 10) * (-1);
            default:
                return 0;
        }
    }
}