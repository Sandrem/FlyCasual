using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;
using RuleSets;
using ActionsList;
using Movement;
using System;
using Tokens;
using System.Linq;
using SubPhases;
using Abilities.SecondEdition;

namespace Ship
{
    namespace G1AStarfighter
    {
        public class P4LOM : G1AStarfighter, ISecondEditionPilot
        {
            public P4LOM() : base()
            {
                PilotName = "4-LOM";
                PilotSkill = 3;
                Cost = 49;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);

                ActionBar.RemovePrintedAction(typeof(FocusAction));
                ActionBar.AddPrintedAction(new CalculateAction());

                UpgradeBar.AddSlot(Upgrade.UpgradeType.Elite);
                PilotAbilities.Add(new P4LOMAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not needed
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class P4LOMAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += Check4LOMMoveAbility;
            Phases.Events.OnEndPhaseStart_Triggers += RegisterEndOfRoundAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= Check4LOMMoveAbility;
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterEndOfRoundAbility;
        }

        // Movement ability begins here
        public void Check4LOMMoveAbility(GenericShip ship)
        {
            if (ship.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AssignCalculateToken);
            }
        }

        public void AssignCalculateToken(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(CalculateToken), Triggers.FinishTrigger);
        }

        // End of round stress pass begins here
        public void RegisterEndOfRoundAbility()
        {
            if (HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, EndOfRoundAbility);
            }
        }

        private void EndOfRoundAbility(object sender, EventArgs e)
        {
            if (TargetsForAbilityExist(FilterAbilityTarget))
            {
                SelectTargetForAbility
                (
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAIAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    true,
                    null,
                    HostShip.PilotName,
                    "Choose a target to transfer stress to.",
                    HostShip.ImageUrl
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SelectAbilityTarget()
        {
            HostShip.Tokens.RemoveToken(typeof(StressToken), delegate
            {
                TargetShip.Tokens.AssignToken(typeof(StressToken), delegate
                {
                    SelectShipSubPhase.FinishSelection();
                });
            });
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterTargetsByRange(ship, 0, 1);
        }

        private int GetAIAbilityPriority(GenericShip ship)
        {
            // AI should stress the most expensive ship enemy ship in range or the least expensive friendly.
            int cost = ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

            if(HostShip.Owner.PlayerNo == ship.Owner.PlayerNo)
            {
                // we're on the same team!
                return  100 - cost;
            }
            else
            {
                // stress the highest costing enemy first
                return 100 + cost;
            }
            
        }

    }
}