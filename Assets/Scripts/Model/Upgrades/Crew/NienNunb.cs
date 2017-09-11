using Upgrade;

namespace UpgradesList
{
    public class NienNunb : GenericUpgrade
    {
        public NienNunb() : base()
        {
            Type = UpgradeType.Crew;
            Name = ShortName = "Nien Nunb";
            ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/0/07/Nien_Nunb.png";
            Cost = 2;
            isUnique = true;
        }

        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Rebels;
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
