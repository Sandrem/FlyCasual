using Upgrade;
using Ship;
using System.Collections.Generic;
using System;

namespace UpgradesList.SecondEdition
{
    public class Q7Astromech : GenericUpgrade
    {
        public Q7Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Q7 Astromech",
                UpgradeType.Astromech,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.Q7AstromechAbility),
                restriction: new FactionRestriction(Faction.Republic)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/75/b9/75b924e8-88e2-4e11-808c-f47f1e2115c2/swz80_upgrade_q7-astromech.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Q7AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckIgnoreObstaclesDuringBarrelRoll += Allow;
            HostShip.OnCheckIgnoreObstaclesDuringBoost += Allow;
        }

        private void Allow(ref bool isAllowed)
        {
            isAllowed = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckIgnoreObstaclesDuringBarrelRoll -= Allow;
            HostShip.OnCheckIgnoreObstaclesDuringBoost -= Allow;
        }
    }
}