using Abilities.SecondEdition;
using ActionsList;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class PoeDameron : T70XWing
        {
            public PoeDameron() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Poe Dameron",
                    6,
                    68,
                    isLimited: true,
                    abilityType: typeof(PoeDameronAbility),
                    charges: 1,
                    regensCharges: true,
                    extraUpgradeIcon: UpgradeType.Talent
                    //seImageNumber: 93
                );

                ModelInfo.SkinName = "Black One";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/ea/60/ea60fada-8e09-4d9e-9dee-c47492e65124/swz25_poe_a1.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an action, you may spend 1 charge to perform an action.
    public class PoeDameronAbility : GenericAbility
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
            if (HostShip.State.Charges > 0)
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
            Messages.ShowInfoToHuman("Poe Dameron: you may spend 1 charge to perform a white action as a red action");

            HostShip.BeforeFreeActionIsPerformed += PayChargeCost;

            List<GenericAction> actions = Selection.ThisShip.GetAvailableActions();
            List<GenericAction> whiteActionBarActionsAsRed = actions
                .Where(n => !n.IsRed)
                .Select(n => n.AsRedAction)
                .ToList();
            HostShip.AskPerformFreeAction(whiteActionBarActionsAsRed, CleanUp);
        }

        private void PayChargeCost(GenericAction action)
        {
            HostShip.State.Charges--;
            HostShip.BeforeFreeActionIsPerformed -= PayChargeCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeFreeActionIsPerformed -= PayChargeCost;
            Triggers.FinishTrigger();
        }

    }
}