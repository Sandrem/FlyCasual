using Ship;
using Ship.AttackShuttle;
using Upgrade;
using System.Linq;
using System;

namespace UpgradesList
{
    public class Phantom : GenericUpgrade
    {
        public Phantom() : base()
        {
            Type = UpgradeType.Title;
            Name = "Phantom";
            Cost = 0;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is AttackShuttle;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);


        }

        public override void Discard(Action callBack)
        {
            

            base.Discard(callBack);
        }

    }
}
