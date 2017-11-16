using System;
using Upgrade;

namespace UpgradesList
{
    public class EzraBridger : GenericUpgrade
    {
        public EzraBridger() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Ezra Bridger";
            Cost = 3;

            isUnique = true;
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            //
        }
    }
}
