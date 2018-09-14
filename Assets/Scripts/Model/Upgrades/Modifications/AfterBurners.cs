
using Ship;
using Upgrade;
using RuleSets;
using System;
using ActionsList;
using System.Collections.Generic;

namespace UpgradesList
{
    public class AfterBurners : GenericUpgrade
    {
        public AfterBurners() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "AfterBurners";
            Cost = 8;

            MaxCharges = 2;
            UsesCharges = true;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.AfterBurnersAbility());

            SEImageNumber = 70;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipBaseSize == BaseSize.Small;
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a speed 3-5 maneuver you may spend 1 charge to perform a boost action, even while stressed.
    public class AfterBurnersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            //AI doesn't use ability
            if (HostShip.Owner.UsesHotacAiRules) return;

            if (HostShip.AssignedManeuver.Speed >= 3 && HostShip.AssignedManeuver.Speed <= 5 && !HostShip.IsBumped && HostUpgrade.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman("AfterBurners: You may spend 1 charge to perform a boost action");

            HostShip.BeforeFreeActionIsPerformed += RegisterSpendChargeTrigger;
            HostShip.AskPerformFreeAction(new BoostAction() { CanBePerformedWhileStressed = true }, CleanUp);
        }

        private void RegisterSpendChargeTrigger(GenericAction action)
        {
            HostShip.BeforeFreeActionIsPerformed -= RegisterSpendChargeTrigger;
            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, delegate { HostUpgrade.SpendCharge(Triggers.FinishTrigger); });
        }

        private void CleanUp()
        {
            HostShip.BeforeFreeActionIsPerformed -= RegisterSpendChargeTrigger;
            Triggers.FinishTrigger();
        }
    }
}

