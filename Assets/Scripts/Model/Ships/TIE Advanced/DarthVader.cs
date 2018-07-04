using System.Collections.Generic;
using Ship;
using RuleSets;
using ActionsList;

// Second->First: Two same actions
// Triggers are empty

namespace Ship
{
    namespace TIEAdvanced
    {
        public class DarthVader : TIEAdvanced, ISecondEditionPilot
        {
            public DarthVader() : base()
            {
                PilotName = "Darth Vader";
                PilotSkill = 9;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Blue";

                PilotAbilities.Add(new Abilities.DarthVaderAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 6;
                MaxForce = 3;
                Cost = 50;

                PilotAbilities.RemoveAll(ability => ability is Abilities.DarthVaderAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.DarthVaderAbility());
            }
        }
    }
}

namespace Abilities
{
    public class DarthVaderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionDecisionSubphaseEnd += DoSecondAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionDecisionSubphaseEnd -= DoSecondAction;
        }

        private void DoSecondAction(GenericShip ship)
        {
            if (!HostShip.Tokens.HasToken(typeof(Tokens.StressToken)) && Phases.CurrentSubPhase.GetType() == typeof(SubPhases.ActionDecisonSubPhase))
            {
                RegisterAbilityTrigger(TriggerTypes.OnFreeActionPlanned, PerformFreeAction);
            }
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            HostShip.GenerateAvailableActionsList();
            List<ActionsList.GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}


namespace Abilities.SecondEdition
{
    //After you perform an action, you may spend 1 force to perform an action.
    public class DarthVaderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action != null && HostShip.Force > 0)
            {
                HostShip.OnActionDecisionSubphaseEnd += DoAnotherAction;
            }
        }

        private void DoAnotherAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= DoAnotherAction;

            if (!ship.Tokens.HasToken(typeof(Tokens.StressToken)) || ship.CanPerformActionsWhileStressed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnFreeAction, PerformAction);
            }
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            HostShip.GenerateAvailableActionsList();
            List<GenericAction> actions = Selection.ThisShip.GetAvailableActionsList();
            HostShip.BeforeFreeActionIsPerformed += PayForceCost;
            Messages.ShowInfo("Darth Vader: you may spend 1 force to perform an action");
            HostShip.AskPerformFreeAction(actions, CleanUp);
        }

        private void PayForceCost(GenericAction action)
        {
            HostShip.Force--;
            HostShip.BeforeFreeActionIsPerformed -= PayForceCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeFreeActionIsPerformed -= PayForceCost;
            Triggers.FinishTrigger();
        }

    }
}