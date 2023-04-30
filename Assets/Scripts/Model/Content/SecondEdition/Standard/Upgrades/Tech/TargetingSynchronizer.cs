using ActionsList;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;
namespace UpgradesList.SecondEdition
{
    public class TargetingSynchronizer : GenericUpgrade
    {
        public TargetingSynchronizer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Synchronizer",
                UpgradeType.Tech,
                cost: 3,
                restriction: new ActionBarRestriction(typeof(TargetLockAction)),
                abilityType: typeof(Abilities.SecondEdition.TargetingSynchronizerAbility)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TargetingSynchronizerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnModifyWeaponAttackRequirementGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnModifyWeaponAttackRequirementGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, GenericSpecialWeapon weapon, ref List<Type> tokenTypes, bool isSilent)
        {
            if (tokenTypes.Contains(typeof(BlueTargetLockToken))
                && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && IsInRangeFromOneToTwo(HostShip, ship)
                && ActionsHolder.HasTargetLockOn(HostShip, Selection.AnotherShip)
            )
            {
                if (!isSilent) Messages.ShowInfo(string.Format("{0}: {1} ignores attack requirement", HostUpgrade.UpgradeInfo.Name, ship.PilotInfo.PilotName));
                tokenTypes.Remove(typeof(BlueTargetLockToken));
            }
        }

        private bool IsInRangeFromOneToTwo(GenericShip ship1, GenericShip ship2)
        {
            DistanceInfo distInfo = new DistanceInfo(ship1, ship2);
            return (distInfo.Range == 1 || distInfo.Range == 2);
        }
    }
}