using Movement;
using Ship;
using SubPhases;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AggressorAssaultFighter
    {
        public class IG88D : AggressorAssaultFighter
        {
            public IG88D() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "IG-88D",
                    4,
                    65,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IG88DAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 200
                );
                
                ModelInfo.SkinName = "Red";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IG88DAbility : GenericAbility
    {
        private string TemporaryAddedManeuver;

        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskToChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskToChangeManeuver;
        }

        private void RegisterAskToChangeManeuver(GenericShip ship)
        {
            if (ship.AssignedManeuver.Bearing == ManeuverBearing.SegnorsLoop)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskToChangeManeuver);
            }
        }

        private void AskToChangeManeuver(object sender, EventArgs e)
        {
            var subphase = Phases.StartTemporarySubPhaseNew<IG88DDecisionSubphase>("IG88D", Triggers.FinishTrigger);

            subphase.InfoText = "You may use anither template of the same speed and direction";

            subphase.AddDecision("Turn", delegate { ChangeBearing(ManeuverBearing.SegnorsLoopUsingTurnTemplate); });
            subphase.AddDecision("Straight", delegate { ChangeBearing(ManeuverBearing.KoiogranTurn); });
            subphase.AddDecision("No changes", delegate { DecisionSubPhase.ConfirmDecision(); });

            subphase.DefaultDecisionName = "No changes";

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            subphase.Start();
        }

        private void ChangeBearing(ManeuverBearing bearing)
        {
            int speed = HostShip.AssignedManeuver.Speed;
            ManeuverDirection direction = HostShip.AssignedManeuver.Direction;

            string newManeuverString = "";
            switch (bearing)
            {
                case ManeuverBearing.KoiogranTurn:
                    newManeuverString = speed + ".F.R";
                    break;
                case ManeuverBearing.SegnorsLoopUsingTurnTemplate:
                    newManeuverString = speed + "." + ((direction == ManeuverDirection.Left) ? "L" : "R") + ".r";
                    break;
            }

            AddTemporaryManeuvers(newManeuverString);
            HostShip.OnMovementExecuted += RemoveTemporaryManeuvers;

            HostShip.SetAssignedManeuver(ShipMovementScript.MovementFromString(newManeuverString));
            DecisionSubPhase.ConfirmDecision();
        }

        private void AddTemporaryManeuvers(string maneuver)
        {
            if (!HostShip.Maneuvers.ContainsKey(maneuver))
            {
                HostShip.Maneuvers.Add(maneuver, MovementComplexity.Complex);
                TemporaryAddedManeuver = maneuver;
            }
            else
            {
                TemporaryAddedManeuver = "";
            }
        }

        private void RemoveTemporaryManeuvers(GenericShip ship)
        {
            if (TemporaryAddedManeuver != "")
            {
                HostShip.Maneuvers.Remove(TemporaryAddedManeuver);
                TemporaryAddedManeuver = "";
            }
        }

        private class IG88DDecisionSubphase : DecisionSubPhase { };
    }
}