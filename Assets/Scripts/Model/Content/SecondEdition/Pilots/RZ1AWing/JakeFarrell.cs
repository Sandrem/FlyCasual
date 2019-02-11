using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class JakeFarrell : RZ1AWing
        {
            public JakeFarrell() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jake Farrell",
                    4,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JakeFarrellAbility),
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent },
                    seImageNumber: 19
                );

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JakeFarrellAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is BoostAction || action is BarrelRollAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, SelectTargetForJakeFarrellAbility);
            }
        }

        private void SelectTargetForJakeFarrellAbility(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                GrantFreeFocusAction,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose a friendly ship, it may perform a free Focus action",
                HostShip
            );
        }

        private void GrantFreeFocusAction()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            Selection.ThisShip = TargetShip;
            TargetShip.AskPerformFreeAction(
                new FocusAction() { HostShip = TargetShip },
                AfterFreeFocusAction
            );
        }

        private void AfterFreeFocusAction()
        {
            Selection.ThisShip = HostShip;
            Triggers.FinishTrigger();
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.This, TargetTypes.OtherFriendly) && FilterTargetsByRange(ship, 0, 1);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = 0;

            if (ship.IsStressed) return priority;

            if (!ship.Tokens.HasToken(typeof(Tokens.FocusToken))) priority += 100;

            priority += ship.PilotInfo.Cost;

            return priority;
        }
    }
}