namespace Assets.Scripts.Car.Upgrades
{
    public abstract class Upgrade
    {
        public string Name
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public double Price
        {
            get; set;
        }

        public abstract void Apply(CarMovement car);
        public abstract void Remove();
    }
}
