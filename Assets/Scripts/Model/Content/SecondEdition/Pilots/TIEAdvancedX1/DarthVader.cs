using Abilities.SecondEdition;
using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class DarthVader : TIEAdvancedX1
        {
            public DarthVader() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Darth Vader",
                    6,
                    65,
                    isLimited: true,
                    abilityType: typeof(DarthVaderAbility),
                    force: 3,
                    extraUpgradeIcon: UpgradeType.Force,
                    seImageNumber: 93
                );

                ModelInfo.SkinName = "Blue";
            }
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
            if (HostShip.State.Force > 0)
            {
                HostShip.OnActionDecisionSubphaseEnd += DoAnotherAction;
            }
        }

        private void DoAnotherAction(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= DoAnotherAction;

            RegisterAbilityTrigger(TriggerTypes.OnFreeAction, PerformAction);
        }

        private void PerformAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Darth Vader: you may spend 1 force to perform an action");

            HostShip.BeforeFreeActionIsPerformed += PayForceCost;

            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
            HostShip.AskPerformFreeAction(actions, CleanUp);
        }

        private void PayForceCost(GenericAction action)
        {
            HostShip.State.Force--;
            HostShip.BeforeFreeActionIsPerformed -= PayForceCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeFreeActionIsPerformed -= PayForceCost;
            Triggers.FinishTrigger();
        }

    }
}
