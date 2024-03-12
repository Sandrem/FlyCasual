﻿using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class JaycrisTubbs : T70XWing
        {
            public JaycrisTubbs() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Jaycris Tubbs",
                    "Loving Father",
                    Faction.Resistance,
                    1,
                    4,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JaycrisTubbsAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Tech,
                        UpgradeType.Astromech,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JaycrisTubbsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += JaycrisTubbsPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= JaycrisTubbsPilotAbility;
        }

        protected void JaycrisTubbsPilotAbility(GenericShip ship)
        {
            if (BoardTools.Board.IsOffTheBoard(ship)) return;
            if (ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, SelectTargetForJaycrisTubbsAbility);
            }
        }

        private void SelectTargetForJaycrisTubbsAbility(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                RemoveStressToken,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Choose a friendly ship, it removes a stress token",
                HostShip
            );
        }

        private void RemoveStressToken()
        {
            TargetShip.Tokens.RemoveToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.This, TargetTypes.OtherFriendly) && FilterTargetsByRange(ship, 0, 1) && ship.Tokens.HasToken(typeof(StressToken));
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = 0;
            
            if (ship.Tokens.HasToken(typeof(StressToken))) priority += 100;

            priority += ship.PilotInfo.Cost;

            return priority;
        }
    }
}