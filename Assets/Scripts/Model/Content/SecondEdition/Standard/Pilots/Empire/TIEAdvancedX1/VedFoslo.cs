using Upgrade;
using Movement;
using Ship;
using System.Collections.Generic;
using Content;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class VedFoslo : TIEAdvancedX1
        {
            public VedFoslo() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Ved Foslo",
                    "Ambitious Engineer",
                    Faction.Imperial,
                    4,
                    4,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VedFosloAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    legality: new List<Legality>
                    {
                        Legality.StandartBanned 
                    },
                    seImageNumber: 95
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VedFosloAbility : GenericAbility
    {
        Dictionary<string, MovementComplexity> defaultManeuvers;
        Dictionary<string, MovementComplexity> abilityManeuvers;

        public override void ActivateAbility()
        {
            HostShip.BeforeMovementIsExecuted += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeMovementIsExecuted -= RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
        }

        private void CleanUpMovements(GenericShip ship) {
            HostShip.Maneuvers = defaultManeuvers;
            HostShip.OnMovementExecuted -= CleanUpMovements;
        }

        private void SetAbilityManeuvers() {
            if (defaultManeuvers == null) {
                defaultManeuvers = new Dictionary<string, MovementComplexity>(HostShip.Maneuvers);
            }
            abilityManeuvers = new Dictionary<string, MovementComplexity>(HostShip.Maneuvers);
            abilityManeuvers["1.L.T"] = MovementComplexity.Normal;
            abilityManeuvers["1.R.T"] = MovementComplexity.Normal;
            abilityManeuvers["3.F.R"] = MovementComplexity.Complex;
            if (HostShip.AssignedManeuver.ToString().Equals("1.F.S"))
            {
                abilityManeuvers["2.F.S"] = MovementComplexity.Normal;
            }
            else
            {
                abilityManeuvers["1.F.S"] = MovementComplexity.Easy;
            }
            if (HostShip.AssignedManeuver.ToString().Equals("3.F.S")) {
                abilityManeuvers["4.F.S"] = MovementComplexity.Easy;
            }
            if (HostShip.AssignedManeuver.ToString().Equals("4.F.S")) {
                abilityManeuvers["3.F.S"] = MovementComplexity.Normal;
            }
            if (HostShip.AssignedManeuver.ToString().Equals("2.L.B")) {
                abilityManeuvers["3.L.B"] = MovementComplexity.Easy;
            }
            if (HostShip.AssignedManeuver.ToString().Equals("2.R.B")) {
                abilityManeuvers["3.R.B"] = MovementComplexity.Easy;
            }
            abilityManeuvers["5.F.R"] = MovementComplexity.Complex;
            abilityManeuvers["2.L.E"] = MovementComplexity.Complex;
            abilityManeuvers["2.R.E"] = MovementComplexity.Complex;
            HostShip.Maneuvers = abilityManeuvers;
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            SetAbilityManeuvers();
            HostShip.OnMovementExecuted += CleanUpMovements;
            HostShip.Owner.ChangeManeuver(ShipMovementScript.SendAssignManeuverCommand, Triggers.FinishTrigger, IsManeuverSameBearing);
        }

        private bool IsManeuverSameBearing(string maneuverString)
        {
            bool result = false;
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if (movementStruct.Bearing == HostShip.AssignedManeuver.Bearing
                && movementStruct.Direction == HostShip.AssignedManeuver.Direction
                && movementStruct.ColorComplexity == HostShip.AssignedManeuver.ColorComplexity
                && (
                    movementStruct.Speed >= HostShip.AssignedManeuver.ManeuverSpeed - 1 && 
                    movementStruct.Speed <= HostShip.AssignedManeuver.ManeuverSpeed + 1)
               ) {
                result = true;
            }
            return result;
        }
    }
}
