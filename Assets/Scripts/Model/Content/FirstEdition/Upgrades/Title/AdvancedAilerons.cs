using Ship;
using Upgrade;
using System.Collections.Generic;
using Movement;

namespace UpgradesList.FirstEdition
{
    public class AdvancedAilerons : GenericUpgrade
    {
        public AdvancedAilerons() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Advanced Ailerons",
                UpgradeType.Title,
                cost: 0,          
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEReaper.TIEReaper)),
                abilityType: typeof(Abilities.FirstEdition.AdvancedAileronsAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class AdvancedAileronsAbility : AdaptiveAileronsAbility
    {

        public override void ActivateAbility()
        {
            base.ActivateAbility();

            HostShip.DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
            HostShip.DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
        }

        public override void DeactivateAbility()
        {
            base.ActivateAbility();

            HostShip.DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Complex);
            HostShip.DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Complex);
        }
    }
}