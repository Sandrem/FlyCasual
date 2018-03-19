using Ship;
using Upgrade;

namespace UpgradesList
{
    public class TwinIonEngineMkII : GenericUpgrade
    {
        public TwinIonEngineMkII() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Twin Ion Engine Mk. II";
            ImageUrl = ImageUrls.GetImageUrl(this, "twin-ion-engine-mkii.png");
            Cost = 1;
            UpgradeAbilities.Add(new Abilities.TreatAllBanksAsGreenAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIE;
        }        
    }
}

namespace Abilities
{
    public class TreatAllBanksAsGreenAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, ref Movement.MovementStruct movement)
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