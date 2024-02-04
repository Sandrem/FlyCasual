using Ship;
using Upgrade;
using SubPhases;
using System;
using BoardTools;
using UnityEngine;
using Movement;
using System.Collections.Generic;
using Content;

namespace UpgradesList.SecondEdition
{
    public class CassianAndor : GenericUpgrade
    {
        public CassianAndor() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cassian Andor",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.CassianAndorCrewAbility),
                seImageNumber: 81,
                legalityInfo: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                }
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(373, 1),
                new Vector2(200, 200)
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
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Enemy).Count > 0) flag = true;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskSelectShip);
            }
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
                HostShip.Owner.ChangeManeuver(
                    AfterManeuverIsChanged,
                    Triggers.FinishTrigger, 
                    delegate { return true; }
                );
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