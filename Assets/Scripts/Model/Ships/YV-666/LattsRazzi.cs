using Arcs;
using BoardTools;
using RuleSets;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace Ship
{
    namespace YV666
    {
        public class LattsRazzi : YV666, ISecondEditionPilot
        {
            public LattsRazzi() : base()
            {
                PilotName = "Latts Razzi";
                PilotSkill = 3;
                Cost = 66;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.LattsRazziPilotAbilitySE());

                SEImageNumber = 212;
            }

            public void AdaptPilotToSecondEdition()
            {
                
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LattsRazziPilotAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.Tokens.HasToken(typeof(BlueTargetLockToken), '*') && IsTargetAvailable())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToSelectLattsRazziTarget);
            }
        }

        private bool IsTargetAvailable()
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (Actions.HasTargetLockOn(HostShip, ship))
                {
                    DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                    if (distInfo.Range == 1 && !HostShip.ShipsBumped.Contains(ship)) return true;
                }
            }

            return false;
        }

        private void AskToSelectLattsRazziTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                TargetIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Choose a ship at range 1 and spend a lock you have on that ship - that ship gains 1 tractor token",
                HostShip.ImageUrl
            );
        }

        private void TargetIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo("Latts Razzi: " + TargetShip.PilotName + " is selected");

            char tlLetter = Actions.GetTargetLocksLetterPair(HostShip, TargetShip);
            HostShip.Tokens.SpendToken(typeof(BlueTargetLockToken), ApplyAbility, tlLetter);
        }

        private void ApplyAbility()
        {
            TargetShip.Tokens.AssignToken(
                new TractorBeamToken(TargetShip, HostShip.Owner),
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            if (!Actions.HasTargetLockOn(HostShip, ship)) return false;

            if (HostShip.ShipsBumped.Contains(ship)) return false;

            if (!FilterTargetsByRange(ship, 1, 1)) return false;

            return true;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.Cost;
        }
    }
}
