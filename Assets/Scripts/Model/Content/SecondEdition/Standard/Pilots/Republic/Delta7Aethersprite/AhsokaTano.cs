using ActionsList;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class AhsokaTano : Delta7Aethersprite
    {
        public AhsokaTano()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Ahsoka Tano",
                "\"Snips\"",
                Faction.Republic,
                3,
                4,
                7,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.AhsokaTanoAbility),
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Ahsoka Tano"
            );

            ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/15f6bf84f63970c982dd722a5473217f.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a maneuver, you may choose a friendly ship at range 0–1 and spend 1 force. 
    //That ship may perform an action, even if it is stressed.
    public class AhsokaTanoAbility : GenericAbility
    {
        protected virtual int ForceCost => 1;
        protected virtual int MinRange => 0;
        protected virtual int MaxRange => 1;

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, SelectTargetForAbility);
        }

        private void SelectTargetForAbility(object sender, EventArgs e)
        {
            if (HostShip.State.Force >= ForceCost && HasTargetsForAbility())
            {
                SelectTargetForAbility(
                    GrantAction,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "Choose a friendly ship, it may perform an action, even if stressed",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool HasTargetsForAbility()
        {
            foreach (GenericShip ship in HostShip.Owner.Ships.Values)
            {
                if (FilterTargets(ship)) return true;
            }

            return false;
        }

        private void GrantAction()
        {
            TargetShip.BeforeActionIsPerformed += PayForceCost;

            SelectShipSubPhase.FinishSelectionNoCallback();
            Selection.ThisShip = TargetShip;

            TargetShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
            TargetShip.OnCanPerformActionWhileStressed += AlwaysAllow;

            var actions = TargetShip.GetAvailableActions();

            TargetShip.AskPerformFreeAction(
                actions,
                delegate {
                    TargetShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;
                    TargetShip.OnCanPerformActionWhileStressed -= AlwaysAllow;

                    Selection.ThisShip = HostShip;
                    TargetShip.BeforeActionIsPerformed -= PayForceCost;
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "You may perform an action, even if you is stressed.",
                HostShip
            );
        }

        private void PayForceCost(GenericAction action, ref bool isFreeAction)
        {
            TargetShip.BeforeActionIsPerformed -= PayForceCost;

            RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, SpendForce);
        }

        private void SpendForce(object sender, EventArgs e)
        {
            HostShip.State.SpendForce(ForceCost, Triggers.FinishTrigger);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.This, TargetTypes.OtherFriendly) && FilterTargetsByRange(ship, MinRange, MaxRange);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = 0;
            
            if (!ship.Tokens.HasToken(typeof(Tokens.FocusToken))) priority += 100;

            priority += ship.PilotInfo.Cost;

            return priority;
        }

        private void ConfirmThatIsPossible(ref bool isAllowed)
        {
            AlwaysAllow(null, ref isAllowed);
        }

        private void AlwaysAllow(GenericAction action, ref bool isAllowed)
        {
            isAllowed = true;
        }
    }
}
