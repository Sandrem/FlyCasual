using Ship;
using Upgrade;
using System.Collections.Generic;
using System;
using SubPhases;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class JabbaTheHutt : GenericUpgrade
    {
        public JabbaTheHutt() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Jabba the Hutt",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                cost: 8,
                charges: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.JabbaTheHuttAbility)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    //During the End Phase, you may choose 1 friendly ship at range 0-2 and spend 1 charge. 
    //If you do, that ship recovers 1 charge on 1 of its equipped illicit upgrades.
    public class JabbaTheHuttAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, Ability);
        }

        private void Ability(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0 && TargetsForAbilityExist(FilterAbilityTarget))
            {
                Messages.ShowInfoToHuman("Jabba the Hutt: Select 1 ship to recover 1 charge on an illicit upgrade.");

                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostName,
                    "Select 1 ship to recover 1 charge on an illicit upgrade.",
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            //prioritize upgrades where all charges are spent. TODO: also prioritize expensive upgrades?
            if (ship.UpgradeBar.GetRechargableUpgrades(UpgradeType.Illicit).Any(u => u.State.Charges == 0)) return 100;
            else return 50;
        }

        private void SelectAbilityTarget()
        {
            var phase = Phases.StartTemporarySubPhaseNew<JabbaTheHuttDecisionSubphase>(
                "Jabba the Hutt: Select upgrade to recover 1 charge", 
                () => {
                    HostUpgrade.State.SpendCharge();
                    Phases.CurrentSubPhase.CallBack();
                });
            phase.TargetShip = TargetShip;
            phase.Start();
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This }) &&
                FilterTargetsByRange(ship, 0, 2) &&
                ship.UpgradeBar.GetRechargableUpgrades(UpgradeType.Illicit).Any();
        }

        protected class JabbaTheHuttDecisionSubphase : DecisionSubPhase
        {
            public GenericShip TargetShip;

            public override void PrepareDecision(Action callBack)
            {
                InfoText = "Jabba the Hutt: Select upgrade to recover 1 charge";

                DecisionViewType = DecisionViewTypes.ImagesUpgrade;

                foreach (var upgrade in TargetShip.UpgradeBar.GetRechargableUpgrades(UpgradeType.Illicit).ToList())
                {
                    AddDecision(upgrade.UpgradeInfo.Name, delegate { RecoverCharge(upgrade); }, upgrade.ImageUrl);
                }

                DefaultDecisionName = GetDecisions().First().Name;

                callBack();
            }

            private void RecoverCharge(GenericUpgrade upgrade)
            {
                upgrade.State.RestoreCharge();
                ConfirmDecision();
            }

        }
    }
}