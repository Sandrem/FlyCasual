using Upgrade;
using Ship;
using Movement;

namespace UpgradesList.FirstEdition
{
    public class TwinIonEngineMkII : GenericUpgrade
    {
        public TwinIonEngineMkII() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Twin Ion Engine Mk. II",
                UpgradeType.Modification,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.TwinIonEngineMkIIAbility)
            );

            // TODOREVERT
            // ImageUrl = ImageUrls.GetImageUrl(this, "twin-ion-engine-mkii.png");
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIE;
        }
    }
}

namespace Abilities.FirstEdition
{
    public class TwinIonEngineMkIIAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.ColorComplexity != MovementComplexity.None)
            {
                if (movement.Bearing == ManeuverBearing.Bank)
                {
                    movement.ColorComplexity = MovementComplexity.Easy;
                }
            }
        }
    }
}