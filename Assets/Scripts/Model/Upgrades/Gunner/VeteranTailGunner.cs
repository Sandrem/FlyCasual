﻿using Upgrade;
using Tokens;
using System.Collections.Generic;
using RuleSets;
using Ship;
using Abilities.SecondEdition;
using ActionsList;
using System.Linq;
using Arcs;

namespace UpgradesList
{
    public class VeteranTailGunner : GenericUpgrade, ISecondEditionUpgrade
    {
        public VeteranTailGunner() : base()
        {
            Types.Add(UpgradeType.Gunner);
            Name = "Veteran Tail Gunner";
            Cost = 4;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new VeteranTailGunnerAbility());

            SEImageNumber = 51;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //Nothing to do, already second edition upgrade
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseArcsType == BaseArcsType.ArcRear;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VeteranTailGunnerAbility : GenericAbility
    {
        // After you perform a primary forward firing arc attack, 
        // you may perform a bonus primary rear firing arc attack.

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Combat.ShotInfo.Weapon != HostShip.PrimaryWeapon) return;
            if (!Combat.ShotInfo.ShotAvailableFromArcs.Any(a => a.Facing == ArcFacing.Forward)) return;

            HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseGunnerAbility);
        }

        private void UseGunnerAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartAdditionalAttack(
                    HostShip,
                    FinishAdditionalAttack,
                    IsRearArcShot,
                    HostUpgrade.Name,
                    "You may perform a bonus primary rear firing arc attack",
                    HostUpgrade.ImageUrl
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.IsAttackPerformed = true;

            Triggers.FinishTrigger();
        }

        private bool IsRearArcShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (Combat.ShotInfo.ShotAvailableFromArcs.Any(a => a.ArcType == ArcTypes.RearAux))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("Attack must use rear firing arc");
            }

            return result;
        }
    }
}