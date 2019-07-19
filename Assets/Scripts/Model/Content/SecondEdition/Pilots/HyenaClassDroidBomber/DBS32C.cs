﻿using System.Collections.Generic;
using Upgrade;
using System.Linq;
using System;
using Ship;
using ActionsList;
using Actions;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class DBS32C : HyenaClassDroidBomber
    {
        public DBS32C()
        {
            PilotInfo = new PilotCardInfo(
                "DBS-32C",
                4,
                42,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DBS32CAbility),
                extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Torpedo, UpgradeType.Missile, UpgradeType.Bomb },
                pilotTitle: "Droid Control Signal Relay"
            );

            ShipInfo.ActionIcons.RemoveActions(typeof(ReloadAction));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(JamAction)));

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/1befc5619a02e2ea8b7bfb8df93471a1.png";
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
                "DBS-32C",
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
                descriptionShort: "DBS-32C",
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