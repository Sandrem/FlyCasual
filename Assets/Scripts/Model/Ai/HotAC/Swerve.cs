using System;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using GameCommands;

namespace AI
{
    public class Swerve
    {
        protected MovementPrediction movementPrediction;
        protected GenericMovement originalMovement;
        protected List<ManeuverHolder> alternativeManeuvers = new List<ManeuverHolder>();
        protected List<ManeuverHolder> failedManeuvers = new List<ManeuverHolder>();

        private bool IsForced;

        public Swerve(bool isForced = false)
        {
            IsForced = isForced;

            originalMovement = Selection.ThisShip.AssignedManeuver;
            alternativeManeuvers = GetAlternativeManeuvers(Selection.ThisShip.AssignedManeuver);
            TryAlternativeMovement();
        }

        protected virtual void TryAlternativeMovement()
        {
            if (alternativeManeuvers.Count > 0)
            {
                ManeuverHolder maneuver = alternativeManeuvers[0];
                maneuver.UpdateColorComplexity();
                alternativeManeuvers.Remove(alternativeManeuvers[0]);

                if (failedManeuvers.Contains(maneuver) || !Selection.ThisShip.HasManeuver(maneuver))
                {
                    TryAlternativeMovement();
                }
                else
                {
                    GenericMovement newMovementAttempt = ShipMovementScript.MovementFromStruct(maneuver);

                    if (DebugManager.DebugAI) Debug.Log("Tries: " + newMovementAttempt);

                    Selection.ThisShip.SetAssignedManeuver(newMovementAttempt);
                    newMovementAttempt.Initialize();
                    movementPrediction = new MovementPrediction(newMovementAttempt, CheckSwerveAlternativePrediction);
                }
            }
            else
            {
                Console.Write("Ship doesn't see alternatives to the asteroid collision", LogTypes.AI, false, "yellow");

                Selection.ThisShip.SetAssignedManeuver(originalMovement);
                LaunchMovementFinally();
            }
        }

        private void LaunchMovementFinally()
        {
            AI.Swerve.GenerateSwerveCommand(Selection.ThisShip.ShipId, Selection.ThisShip.AssignedManeuver.ToString());
            Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
        }

        protected virtual void CheckSwerveAlternativePrediction()
        {
            if ((movementPrediction.AsteroidsHit.Count == 0) && (!movementPrediction.IsOffTheBoard))
            {
                if (DebugManager.DebugAI) Debug.Log("And it works!");
                Console.Write("Ship found maneuver to avoid asteroid collision\n", LogTypes.AI, true, "yellow");

                alternativeManeuvers = new List<ManeuverHolder>();

                GenericMovement assignedManeuver = movementPrediction.CurrentMovement;
                assignedManeuver.movementPrediction = movementPrediction;

                Selection.ThisShip.SetAssignedManeuver(assignedManeuver);
                LaunchMovementFinally();
            }
            else
            {
                if (DebugManager.DebugAI) Debug.Log("But it doesn't works...");
                if (IsForced)
                {
                    if (DebugManager.DebugAI) Debug.Log("Search for new alternatives...");
                    GetAlternativeManeuvers(movementPrediction.CurrentMovement);
                }
                TryAlternativeMovement();
            }
        }

        protected virtual List<ManeuverHolder> GetAlternativeManeuvers(GenericMovement maneuver)
        {
            ManeuverHolder movementStruct = new ManeuverHolder
            (
                maneuver.ManeuverSpeed,
                maneuver.Direction,
                maneuver.Bearing,
                maneuver.ColorComplexity
            );

            if (IsForced)
            {
                if (!failedManeuvers.Contains(movementStruct)) failedManeuvers.Add(movementStruct);
            }

            ManeuverHolder alternativeMovementStruct = movementStruct;

            switch (maneuver.Bearing)
            {
                case ManeuverBearing.Straight:
                    alternativeMovementStruct.Bearing = ManeuverBearing.Bank;

                    alternativeMovementStruct.Direction = ManeuverDirection.Left;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));

                    alternativeMovementStruct.Direction = ManeuverDirection.Right;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));
                    break;
                case ManeuverBearing.Bank:
                    alternativeMovementStruct.Bearing = ManeuverBearing.Turn;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));

                    alternativeMovementStruct.Bearing = ManeuverBearing.Straight;
                    alternativeMovementStruct.Direction = ManeuverDirection.Forward;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));
                    break;
                case ManeuverBearing.Turn:
                    alternativeMovementStruct.Bearing = ManeuverBearing.Bank;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));
                    break;
                case ManeuverBearing.KoiogranTurn:
                    alternativeMovementStruct.Bearing = ManeuverBearing.Bank;

                    alternativeMovementStruct.Direction = ManeuverDirection.Left;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));

                    alternativeMovementStruct.Direction = ManeuverDirection.Right;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));
                    break;
                default:
                    break;
            }

            return alternativeManeuvers;
        }

        protected ManeuverHolder GetSimilarManeuverByStruct(ManeuverHolder alternativeManeuverStruct)
        {
            ManeuverHolder result = alternativeManeuverStruct;

            if (!Selection.ThisShip.HasManeuver(result) || (failedManeuvers.Contains(result)))
            {
                if (result.Speed == ManeuverSpeed.Speed1)
                {
                    result.SpeedInt++;
                }
                else
                {
                    result.SpeedInt--;
                    if (!Selection.ThisShip.HasManeuver(result) || (failedManeuvers.Contains(result)))
                    {
                        result.SpeedInt--;
                        if (!Selection.ThisShip.HasManeuver(result) || (failedManeuvers.Contains(result)))
                        {
                            result.SpeedInt--;  //for example, 5 bank -> 2 bank
                        }
                    }
                }
            }

            return result;
        }

        public static void GenerateSwerveCommand(int shipId, string maneuverCode)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("id", shipId.ToString());
            parameters.AddField("maneuver", maneuverCode);
            GameCommand command = new HotacSwerveCommand(
                GameCommandTypes.HotacSwerve,
                typeof(SubPhases.ActivationSubPhase),
                parameters.ToString()
            );

            ReplaysManager.RecordCommand(command);

            parameters = new JSONObject();
            parameters.AddField("id", shipId.ToString());
            command = new AssignManeuverCommand(
                GameCommandTypes.ActivateAndMove,
                typeof(SubPhases.ActivationSubPhase),
                parameters.ToString()
            );

            ReplaysManager.RecordCommand(command);
        }
    }
}

