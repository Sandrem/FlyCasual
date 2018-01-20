﻿using Upgrade;

namespace UpgradesList
{
    public class NienNunb : GenericUpgrade
    {
        public NienNunb() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Nien Nunb";
            Cost = 1;
            isUnique = true;
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGetManeuverColorDecreaseComplexity += NienNunbAbility;
        }

        private void NienNunbAbility(Ship.GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.ManeuverColor.None)
            {
                if (movement.Bearing == Movement.ManeuverBearing.Straight)
                {
                    movement.ColorComplexity = Movement.ManeuverColor.Green;
                }
            }
        }
    }
}
