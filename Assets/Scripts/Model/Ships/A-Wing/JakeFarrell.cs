using Abilities;
using ActionsList;
using RuleSets;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    namespace AWing
    {
        public class JakeFarrell : AWing, ISecondEditionPilot
        {
            public JakeFarrell() : base()
            {
                PilotName = "Jake Farrell";
                PilotSkill = 7;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Blue";

                PilotAbilities.Add(new JakeFarrellAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 40;

                PilotAbilities.RemoveAll(a => a is JakeFarrellAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.JakeFarrellAbilitySE());

                SEImageNumber = 19;
            }
        }
    }
}

namespace Abilities
{
    public class JakeFarrellAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterJakeFarrellAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterJakeFarrellAbility;
        }

        private void RegisterJakeFarrellAbility(GenericShip ship, Type tokenType)
        {
            if (tokenType == typeof(Tokens.FocusToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, FreeRepositionAction);
            }
        }

        private void FreeRepositionAction(object sender, EventArgs e)
        {
            GenericShip originalSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;
            List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.BoostAction(), new ActionsList.BarrelRollAction() };
            HostShip.AskPerformFreeAction(
                actions,
                delegate ()
                {
                    Selection.ThisShip = originalSelectedShip;
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JakeFarrellAbilitySE : GenericAbility
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
                if (IsAbilityUsed) return;
                IsAbilityUsed = true;

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
                HostShip.PilotName,
                "Choose a friendly ship, it may perform a free Focus action",
                HostShip.ImageUrl,
                true
            );
        }

        private void GrantFreeFocusAction()
        {
            TargetShip.AskPerformFreeAction(
                new FocusAction() { Host = TargetShip },
                SelectShipSubPhase.FinishSelection
            );
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

            priority += ship.Cost;

            return priority;
        }
    }
}