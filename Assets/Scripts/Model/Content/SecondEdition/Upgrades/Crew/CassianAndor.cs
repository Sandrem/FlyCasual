﻿using Ship;
using Upgrade;
using SubPhases;
using System;
using BoardTools;
using UnityEngine;
using Movement;

namespace UpgradesList.SecondEdition
{
    public class CassianAndor : GenericUpgrade
    {
        public CassianAndor() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cassian Andor",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.CassianAndorCrewAbility),
                seImageNumber: 81
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class CassianAndorCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsAbilityActivationGenerateListeners += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivationGenerateListeners -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(1,2), Team.Type.Enemy).Count > 0)
            {
                HostShip.OnSystemsAbilityActivation += RegisterAbility;
            }
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskSelectShip);
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void AskSelectShip(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            SelectTargetForAbility(
                GuessManeuver,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                "Cassian Andor",
                "You may choose 1 enemy ship to guess a bearing and maneuver. If it is true, you may change own maneuver.",
                imageSource: HostUpgrade
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.Enemy) && FilterTargetsByRange(ship, 1, 2);
        }

        private void GuessManeuver()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            GuessManeuverScript.Initialize(ManeuverDataIsEntered);
        }

        private void ManeuverDataIsEntered(ManeuverBearing bearing, ManeuverSpeed speed)
        {
            if (TargetShip.AssignedManeuver != null) Roster.ToggleManeuverVisibility(TargetShip, true);

            if (TargetShip.AssignedManeuver != null && TargetShip.AssignedManeuver.Bearing == bearing && TargetShip.AssignedManeuver.ManeuverSpeed == speed)
            {
                Messages.ShowInfo("Maneuver is match your guess, you can change own manevuer");
                HostShip.Owner.ChangeManeuver(AfterManeuverIsChanged, delegate { return true; });
            }
            else
            {
                Messages.ShowInfo("Chosen ship has another maneuver");
                Triggers.FinishTrigger();
            }
        }

        private void AfterManeuverIsChanged(string maneuverString)
        {
            HostShip.SetAssignedManeuver(ShipMovementScript.MovementFromString(maneuverString));

            Triggers.FinishTrigger();
        }
    }
}