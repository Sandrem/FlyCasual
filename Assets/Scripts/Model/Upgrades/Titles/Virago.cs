using Abilities.SecondEdition;
using ActionsList;
using RuleSets;
using Ship;
using Ship.StarViper;
using SquadBuilderNS;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList
{
    public class Virago : GenericUpgradeSlotUpgrade, ISecondEditionUpgrade
    {
        public Virago() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Virago";
            Cost = 1;
            isUnique = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.System),
                new UpgradeSlot(UpgradeType.Illicit)
            };
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 10;
            AddedSlots.Clear(); //Illicit and system slots are already on base ships.
            AddedSlots.Add(new UpgradeSlot(UpgradeType.Modification));
            UpgradeAbilities.Add(new ViragoAbility());
            MaxCharges = 2; //Charges for boost ability
        }


        public override void PreAttachToShip(GenericShip host)
        {
            if (RuleSet.Instance is SecondEdition)
            {
                base.PreAttachToShip(host);

                Host.MaxShields++;
            }
        }

        public override void PreDettachFromShip()
        {
            if (RuleSet.Instance is SecondEdition)
            {
                base.PreDettachFromShip();

                Host.MaxShields--;
            }
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (RuleSet.Instance is FirstEdition)
            {
                return ((ship is StarViper) && (ship.PilotSkill > 3));
            }
            else
            {
                return ship is StarViper;
            }
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            if (RuleSet.Instance is FirstEdition)
            {
                bool result = Host.PilotSkill > 3;
                if (!result) Messages.ShowError("You cannot equip \"Virago\" if pilot's skill is \"3\" or lower");
                return result;
            }
            else return true;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ViragoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostUpgrade.Charges > 0 && !HostShip.IsStressed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            Messages.ShowInfo("Virago: You may spend 1 charge to perform a red boost action. There are " + HostUpgrade.Charges + " charges remaining.");
            HostShip.BeforeFreeActionIsPerformed += RegisterSpendChargeTrigger;
            Selection.ChangeActiveShip(HostShip);
            HostShip.AskPerformFreeAction(new BoostAction() { IsRed = true, CanBePerformedWhileStressed = false }, CleanUp);
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
