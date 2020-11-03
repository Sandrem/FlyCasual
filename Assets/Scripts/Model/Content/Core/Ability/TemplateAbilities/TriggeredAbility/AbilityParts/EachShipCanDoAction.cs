using Abilities.Parameters;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Abilities
{
    public class EachShipCanDoAction : AbilityPart
    {
        private TriggeredAbility Ability;

        private Action<GenericShip, Action> EachShipAction;
        private Action OnFinish;
        private ConditionsBlock Conditions;
        private AbilityDescription AbilityDescription;

        private List<GenericShip> AlreadyActivatedShips = new List<GenericShip>();

        public EachShipCanDoAction(
            Action<GenericShip, Action> eachShipAction,
            Action onFinish = null,
            ConditionsBlock conditions = null,
            AbilityDescription description = null)
        {
            EachShipAction = eachShipAction;
            OnFinish = onFinish;
            Conditions = conditions;
            AbilityDescription = description;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;
            AlreadyActivatedShips = new List<GenericShip>();

            StartSelection();
        }

        private void StartSelection()
        {
            if (Ability.TargetsForAbilityExist(FilterTargets))
            {
                Ability.SelectTargetForAbility(
                    WhenShipIsSelected,
                    FilterTargets,
                    GetAiPriority,
                    Ability.HostShip.Owner.PlayerNo,
                    AbilityDescription.Name,
                    AbilityDescription.Description,
                    AbilityDescription.ImageSource,
                    showSkipButton: true,
                    callback: AfterShipIsSelected
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void WhenShipIsSelected()
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Ability.RegisterAbilityTrigger
            (
                TriggerTypes.OnAbilityDirect,
                DoEachShipActon
            );
            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, StartSelection);
        }

        private bool FilterTargets(GenericShip ship)
        {
            ConditionArgs args = new ConditionArgs()
            {
                ShipToCheck = ship,
                ShipAbilityHost = Ability.HostShip
            };

            return Conditions.Passed(args) && !AlreadyActivatedShips.Contains(ship);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }

        private void DoEachShipActon(object sender, EventArgs e)
        {
            AlreadyActivatedShips.Add(Ability.TargetShip);
            EachShipAction(Ability.TargetShip, Triggers.FinishTrigger);
        }

        private void AfterShipIsSelected()
        {
            Triggers.FinishTrigger();
        }
    }
}
