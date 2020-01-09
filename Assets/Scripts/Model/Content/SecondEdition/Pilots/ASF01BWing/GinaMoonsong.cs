﻿using Abilities.SecondEdition;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class GinaMoonsong : ASF01BWing
        {
            public GinaMoonsong() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gina Moonsong",
                    5,
                    50,
                    isLimited: true,
                    abilityType: typeof(GinaMoonsongAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/32/8a/328a4f31-c01e-4966-a418-59c6fd42739e/swz66_gina-moonsong.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagement Phase, you must transfer 1 of your stress tokens to another friendly ship at range 0-2.
    public class GinaMoonsongAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }
        protected virtual void CheckAbility()
        {

            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, SelectTarget);
        }

        protected void SelectTarget(object sender, EventArgs e)
        {
            if (!HostShip.IsStressed ||
                (HostShip.Owner.Ships.Count == 1) ||
                (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(0, 2), Team.Type.Friendly).Count == 0))
            {
                Triggers.FinishTrigger();
                return;
            }
            
            SelectTargetForAbility(
                TargetIsSelected,
                FilterAbilityTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Transfer 1 of your stress tokens to a friendly ship at range 0-2",
                HostShip, 
                showSkipButton: false
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            var pri = ship.GetAIStressPriority();
            return (pri * 1000) + (pri * ship.PilotInfo.Cost);
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 0, 2);
        }

        private void TargetIsSelected()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            HostShip.Tokens.RemoveToken(typeof(StressToken), () =>
            {
                TargetShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
            });
        }
    }
}
