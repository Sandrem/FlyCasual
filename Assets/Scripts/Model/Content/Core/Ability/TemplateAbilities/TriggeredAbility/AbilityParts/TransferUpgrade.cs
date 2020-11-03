using Movement;
using System;
using UnityEngine;
using Upgrade;

namespace Abilities
{
    public class TransferUpgradeAction : AbilityPart
    {
        private TriggeredAbility Ability;

        public TransferUpgradeAction()
        {
            
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            if (TargetUpgrade.IsAllowedForShip(Ability.TargetShip) && TargetUpgrade.UpgradeInfo.Restrictions.IsAllowedForShip(Ability.TargetShip))
            {
                TargetUpgrade.DettachFromShip();
                TargetUpgrade.PreDettachFromShip();
                TargetUpgrade.PreAttachToShip(Ability.TargetShip);
                TargetUpgrade.AttachToShip(Ability.TargetShip);

                Roster.UpdateUpgradesPanel(Ability.TargetShip, Ability.TargetShip.InfoPanel);
                Roster.UpdateUpgradesPanel(Ability.HostShip, Ability.HostShip.InfoPanel);
            }
            else
            {
                Messages.ShowError("This upgrade cannot be installed on this ship");
            }
            Triggers.FinishTrigger();
        }
    }
}
