using Upgrade;
using UnityEngine;
using Ship;
using System;

namespace UpgradesList.SecondEdition
{
    public class R5TK : GenericUpgrade
    {
        public R5TK() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R5-TK",
                UpgradeType.Astromech,
                cost: 1,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R5TKAbility),
                restriction: new FactionRestriction(Faction.Scum),
                seImageNumber: 145
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R5TKAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryAttackSameTeamCheck += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryAttackSameTeamCheck -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool result)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) result = true;
        }
    }
}