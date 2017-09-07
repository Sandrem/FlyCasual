using Ship;
using Upgrade;

namespace UpgradesList
{
    public class TwinIonEngineMkII : GenericUpgrade
    {
        public TwinIonEngineMkII() : base()
        {
            Type = UpgradeType.Modification;
            Name = "Twin Ion Engine Mk. II";
            ShortName = "TIE MkII";
            ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/5/51/Twin-ion-engine-mk2-1-.png";
            Cost = 1;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIE;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGetManeuverColorDecreaseComplexity += TwinIonEngineMkIIAbility;
        }

        private void TwinIonEngineMkIIAbility(GenericShip ship, ref Movement.MovementStruct movement)
        {
            if (movement.ColorComplexity != Movement.ManeuverColor.None)
            {
                if (movement.Bearing == Movement.ManeuverBearing.Bank)
                {
                    movement.ColorComplexity = Movement.ManeuverColor.Green;
                }
            }
        }
    }
}
