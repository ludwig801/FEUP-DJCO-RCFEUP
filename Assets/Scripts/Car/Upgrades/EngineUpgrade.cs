using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Car.Upgrades
{
    public class EngineUpgrade : Upgrade
    {
        public int TopSpeedIncrement;

        public override void Apply(CarMovement car)
        {
            car.TopSpeed += TopSpeedIncrement;
        }

        public override void Remove()
        {
            throw new NotImplementedException();
        }
    }
}
