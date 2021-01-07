using Abilities.Parameters;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Abilities
{
    public class EachUpgradeCanDoAction : AbilityPart
    {
        private GenericAbility Ability;

        private AbilityPart EachUpgradeAction;
        private Action OnFinish;
        private ConditionsBlock Conditions;

        private List<GenericUpgrade> UpgradesThatCanBeTransfered = new List<GenericUpgrade>();

        public EachUpgradeCanDoAction(
            AbilityPart eachUpgradeAction,
            Action onFinish = null,
            ConditionsBlock conditions = null
        )
        {
            EachUpgradeAction = eachUpgradeAction;
            OnFinish = onFinish;
            Conditions = conditions;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;
            UpgradesThatCanBeTransfered = GetUpgradesFiltered();

            StartSelection();
        }

        private List<GenericUpgrade> GetUpgradesFiltered()
        {
            return Ability.HostShip.UpgradeBar.GetUpgradesAll().Where(n => FilterTargets(n)).ToList();
        }

        private bool FilterTargets(GenericUpgrade upgrade)
        {
            ConditionArgs args = new ConditionArgs()
            {
                UpgradeToCheck = upgrade
            };

            return Conditions.Passed(args);
        }

        private void StartSelection()
        {
            if (UpgradesThatCanBeTransfered.Count > 0)
            {
                Selection.ChangeActiveShip(Ability.HostShip);
                StartDecisionSubphase();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void StartDecisionSubphase()
        {
            SelectUpgradeDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<SelectUpgradeDecisionSubphase>("Choose an upgrade", UpgradeIsSelectedFinish);

            subphase.DescriptionShort = "Choose an upgrade";
            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
            subphase.DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            foreach (GenericUpgrade upgrade in UpgradesThatCanBeTransfered)
            {
                subphase.AddDecision(upgrade.UpgradeInfo.Name, delegate { UpgradeIsSelected(upgrade); }, upgrade.ImageUrl);
            }

            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
            subphase.ShowSkipButton = true;

            subphase.Start();
        }

        private void UpgradeIsSelected(GenericUpgrade upgrade)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            UpgradesThatCanBeTransfered.Remove(upgrade);

            EachUpgradeAction.TargetUpgrade = upgrade;

            Ability.RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, delegate { EachUpgradeAction.DoAction(Ability); });
            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, StartSelection);
        }

        private void UpgradeIsSelectedFinish()
        {
            Triggers.FinishTrigger();
        }

        /*private void WhenUpgradeIsSelected()
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Ability.RegisterAbilityTrigger
            (
                TriggerTypes.OnAbilityDirect,
                DoEachUpgradeActon
            );
            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, StartSelection);
        }*/

        private void DoEachUpgradeActon(object sender, EventArgs e)
        {
            //UpgradesThatCanBeTransfered.Remove(Ability.TargetUpgrade);
            //EachUpgradeAction(Ability.TargetUpgrade, Triggers.FinishTrigger);
        }

        private class SelectUpgradeDecisionSubphase : DecisionSubPhase { }
    }
}
