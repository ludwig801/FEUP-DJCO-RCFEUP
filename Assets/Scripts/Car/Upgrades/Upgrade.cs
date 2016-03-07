public class Upgrade
{
    public int UpgradeId;

    public int Level;

    public int Increment;

    public double PriceForNextLevel;

    public int GetIncrementByLevel()
    {
        switch(UpgradeId)
        {
            case 1:
                return Level * 10;
            case 2:
                return Level * 10;
            case 3:
                return (Level * 10) * (-1);
            default:
                return 0;
        }
    }

}