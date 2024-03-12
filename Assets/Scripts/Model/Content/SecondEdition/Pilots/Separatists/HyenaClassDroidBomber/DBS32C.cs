﻿using Actions;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class DBS32C : HyenaClassDroidBomber
    {
        public DBS32C()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DBS-32C",
                "Droid Control Signal Relay",
                Faction.Separatists,
                3,
                4,
                16,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DBS32CAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Sensor,
                    UpgradeType.TacticalRelay,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ShipInfo.ActionIcons.RemoveActions(typeof(ReloadAction));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(JamAction), ActionColor.Red));
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DBS32CAbility : GenericAbility
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
            if (HostShip.Tokens.HasToken(typeof(Tokens.CalculateToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                UseDbs32cAbility,
                descriptionLong: "You may spend 1 calculate token to coordinate ship with Networked Calculations ability",
                imageHolder: HostShip,
                showSkipButton: true,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void UseDbs32cAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.SpendToken(
                typeof(Tokens.CalculateToken),
                PerformLimitedCoordinateAction
            );
        }

        private void PerformLimitedCoordinateAction()
        {
            HostShip.OnCheckCanCoordinate += ForbitLimitedShips;

            HostShip.AskPerformFreeAction(
                new CoordinateAction(),
                FinishAbility,
                descriptionShort: HostShip.PilotInfo.PilotName,
                isForced: true
            );
        }

        private void ForbitLimitedShips(GenericShip ship, ref bool canBeCoordinated)
        {
            if (!ship.ShipAbilities.Any(n => n is NetworkedCalculationsAbility)) canBeCoordinated = false;
        }

        private void FinishAbility()
        {
            HostShip.OnCheckCanCoordinate -= ForbitLimitedShips;
            Triggers.FinishTrigger();
        }
    }
}