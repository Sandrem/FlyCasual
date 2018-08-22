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
        protected List<MovementStruct> alternativeManeuvers = new List<MovementStruct>();
        protected List<MovementStruct> failedManeuvers = new List<MovementStruct>();

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
                MovementStruct maneuver = alternativeManeuvers[0];
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

                alternativeManeuvers = new List<MovementStruct>();

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

        protected virtual List<MovementStruct> GetAlternativeManeuvers(GenericMovement maneuver)
        {
            MovementStruct movementStruct = new MovementStruct
            {
                Speed = maneuver.ManeuverSpeed,
                Bearing = maneuver.Bearing,
                Direction = maneuver.Direction,
                ColorComplexity = maneuver.ColorComplexity
            };

            if (IsForced)
            {
                if (!failedManeuvers.Contains(movementStruct)) failedManeuvers.Add(movementStruct);
            }

            MovementStruct alternativeMovementStruct = movementStruct;

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

        protected MovementStruct GetSimilarManeuverByStruct(MovementStruct alternativeManeuverStruct)
        {
            MovementStruct result = alternativeManeuverStruct;

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
            GameCommand command = new AssignManeuverCommand(
                GameCommandTypes.AssignManeuver,
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

