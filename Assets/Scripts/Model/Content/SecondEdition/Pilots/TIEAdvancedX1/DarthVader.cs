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
                    67,
                    isLimited: true,
                    abilityType: typeof(DarthVaderAbility),
                    force: 3,
                    extraUpgradeIcon: UpgradeType.ForcePower,
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
            HostShip.BeforeActionIsPerformed += PayForceCost;

            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();

            HostShip.AskPerformFreeAction(
                actions,
                CleanUp,
                HostShip.PilotInfo.PilotName,
                "After you perform an action, you may spend 1 Force to perform an action",
                HostShip
            );
        }

        private void PayForceCost(GenericAction action, ref bool isFreeAction)
        {
            HostShip.State.Force--;
            HostShip.BeforeActionIsPerformed -= PayForceCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= PayForceCost;
            Triggers.FinishTrigger();
        }

    }
}
