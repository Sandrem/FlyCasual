using Ship;
using Upgrade;
using System.Collections.Generic;
using Movement;

namespace UpgradesList.FirstEdition
{
    public class LightScykInterceptor : GenericUpgrade
    {
        public LightScykInterceptor() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Light Scyk\" Interceptor",
                UpgradeType.Title,
                cost: -2,
                forbidSlot: UpgradeType.Modification,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.M3AInterceptor.M3AInterceptor)),
                abilityType: typeof(Abilities.FirstEdition.LightScykInterceptorAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class LightScykInterceptorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckFaceupCrit += HostShip_OnCheckFaceupCrit;
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckFaceupCrit -= HostShip_OnCheckFaceupCrit;
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckAbility;
        }

        private void HostShip_OnCheckFaceupCrit(ref bool faceUp)
        {
            faceUp = true;
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
