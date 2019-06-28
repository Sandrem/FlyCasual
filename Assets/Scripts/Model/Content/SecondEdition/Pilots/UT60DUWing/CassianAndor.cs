﻿using Abilities.SecondEdition;
using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class CassianAndor : UT60DUWing
        {
            public CassianAndor() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cassian Andor",
                    3,
                    51,
                    isLimited: true,
                    abilityType: typeof(CassianAndorAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 56
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CassianAndorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, Ability);
        }

        protected virtual string GenerateAbilityMessage()
        {
            return "Choose a ship to remove a stress token from.";
        }

        private void Ability(object sender, EventArgs e)
        {
            bool friendlyShipIsStressed = false;

            foreach (var kv in HostShip.Owner.Ships)
            {
                GenericShip ship = kv.Value;
                if (ship.Tokens.HasToken(typeof(StressToken)))
                {
                    friendlyShipIsStressed = true;
                }
            }

            if (friendlyShipIsStressed)
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    GenerateAbilityMessage(),
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                ship.Tokens.HasToken(typeof(StressToken));
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            if (ActionsHolder.HasTarget(ship)) result += 100;
            return result;
        }

        private void SelectAbilityTarget()
        {
            TargetShip.Tokens.RemoveToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
        }
    }
}
