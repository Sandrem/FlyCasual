using Abilities.SecondEdition;
using ActionsList;
using RuleSets;
using Ship;
using System;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class LieutenantSai : LambdaShuttle, ISecondEditionPilot
        {
            public LieutenantSai() : base()
            {
                PilotName = "Lieutenant Sai";
                PilotSkill = 3;
                Cost = 47;
                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new LieutenantSaiAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                // nah boyeee
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantSaiAbility : GenericAbility
    {
        GenericShip abilityTarget;
        GenericAction abilityAction;
        public override void ActivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected += RegisterAbilityEvents;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCoordinateTargetIsSelected -= RegisterAbilityEvents;
        }

        private void RegisterAbilityEvents(GenericShip targetShip)
        {
            abilityTarget = targetShip;
            targetShip.OnActionIsPerformed += RegisterAbility;
            targetShip.OnActionDecisionSubphaseEndNoAction += DeregisterAbilityEvents;
        }

        private void DeregisterAbilityEvents(GenericShip ship)
        {
            abilityTarget.OnActionIsPerformed -= RegisterAbility;
            abilityTarget.OnActionDecisionSubphaseEndNoAction -= DeregisterAbilityEvents;
            abilityTarget = null;
            abilityAction = null;
        }

        private void RegisterAbility(GenericAction action)
        {

            DeregisterAbilityEvents(abilityTarget);

            if (action == null || !HostShip.ActionBar.HasAction(action.GetType()))
            {
                return;
            }

            abilityAction = action;
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Lieutenant Sai's ability",
                TriggerType = TriggerTypes.OnActionIsPerformed,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = AbilityTakeFreeAction
            });
        }

        private void AbilityTakeFreeAction(object sender, EventArgs e)
        {
            GenericShip previousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);
            HostShip.AskPerformFreeAction(abilityAction, delegate 
            {
                Selection.ChangeActiveShip(previousActiveShip);
                Triggers.FinishTrigger();
            });
        }
    }
}
