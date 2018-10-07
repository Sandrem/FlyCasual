using Upgrade;
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
    public class VeteranTurretGunner : GenericUpgrade, ISecondEditionUpgrade
    {
        public VeteranTurretGunner() : base()
        {
            Types.Add(UpgradeType.Gunner);
            Name = "Veteran Turret Gunner";
            Cost = 8;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new VeteranTurretGunnerAbility());

            SEImageNumber = 52;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //Nothing to do, already second edition upgrade
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ActionBar.HasAction(typeof(RotateArcAction));
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VeteranTurretGunnerAbility : GenericAbility
    {
        // After you perform a primary attack, you may perform a bonus turret arc
        // attack using a turret arc you did not already attack from this round.

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

            bool availableArcsArePresent = HostShip.ArcInfo.Arcs.Any(a => a.ArcType == ArcTypes.Mobile && !a.WasUsedForAttackThisRound);
            if (availableArcsArePresent)
            {
                HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
            }
            else
            {
                Messages.ShowError(HostUpgrade.Name + ": No arc to use");
            }
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
                    IsUnusedTurretArcShot,
                    HostUpgrade.Name,
                    "You may perform a bonus turret arc attack using another turret arc",
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

        private bool IsUnusedTurretArcShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (Combat.ShotInfo.ShotAvailableFromArcs.Any(a => a.ArcType == ArcTypes.Mobile && !a.WasUsedForAttackThisRound))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("Attack must use a turret arc you did not already attack from this round");
            }

            return result;
        }
    }
}