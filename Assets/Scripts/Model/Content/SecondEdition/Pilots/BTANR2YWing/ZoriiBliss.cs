using ActionsList;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class ZoriiBliss : BTANR2YWing
        {
            public ZoriiBliss() : base()
            {
                PilotInfo = new PilotCardInfo
                (
                    "Zorii Bliss",
                    5,
                    35,
                    extraUpgradeIcon: UpgradeType.Talent,
                    isLimited: true,
                    charges: 1,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.ZoriiBlissAbility)
                );

                ImageUrl = "https://i.imgur.com/gzY2RZJ.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZoriiBlissAbility : GenericAbility
    {
        GenericAction ActionPerformedNearby;

        public override void ActivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnActionIsPerformedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (HostShip.State.Charges > 0
                && HostShip.ActionBar.HasAction(action.GetType())
                && IsPerformActionStep()
                && IsInRange1()
                && !HostShip.IsAlreadyExecutedAction(action)) // To track is action performed - to spend charge correctly
            {
                ActionPerformedNearby = action;
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToPerformActionAsRed);
            }
        }

        private bool IsPerformActionStep()
        {
            return Phases.CurrentSubPhase.GetType() == typeof(SubPhases.ActionDecisonSubPhase);
        }

        private bool IsInRange1()
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, Selection.ThisShip);
            return distInfo.Range == 1;
        }

        private void AskToPerformActionAsRed(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            GenericAction sameActionAsRed = ActionPerformedNearby.AsRedAction;
            sameActionAsRed.HostShip = HostShip;

            HostShip.AskPerformFreeAction
            (
                sameActionAsRed,
                FinishAbility,
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "You may perform action, treating it as red",
                imageHolder: HostShip
            );
        }

        private void FinishAbility()
        {
            if (HostShip.IsAlreadyExecutedAction(ActionPerformedNearby)) HostShip.SpendCharge();
            Selection.ChangeActiveShip(ActionPerformedNearby.HostShip);

            Triggers.FinishTrigger();
        }
    }
}
