using Arcs;
using Movement;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class RepulsorliftStabilizers : GenericUpgrade
    {
        public RepulsorliftStabilizers() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Repulsorlift Stabilizers",
                UpgradeType.Configuration,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.HMPDroidGunship.HMPDroidGunship)),
                abilityType: typeof(Abilities.SecondEdition.RepulsorliftStabilizersActiveAbility)
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b9/24/b92420be-2835-4e12-b76e-b2675533249c/swz71_upgrade_stabilizer-active.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RepulsorliftStabilizersActiveAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterYourRevealManeuver
        (
            ifBank: true,
            ifTurn: true
        );

        public override AbilityPart Action => new ChangeManeuverAction
        (
            changeToSideslip: true
        );
    }
}