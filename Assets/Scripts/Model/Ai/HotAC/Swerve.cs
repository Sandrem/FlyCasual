using System;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace AI
{
    public class Swerve
    {
        protected MovementPrediction movementPrediction;
        protected List<MovementStruct> alternativeManeuvers = new List<MovementStruct>();
        protected List<MovementStruct> failedManeuvers = new List<MovementStruct>();

        private bool IsForced;

        public Swerve(bool isForced = false)
        {
            IsForced = isForced;

            alternativeManeuvers = GetAlternativeManeuvers(Selection.ThisShip.AssignedManeuver);
            TryAlternativeMovement();
        }

        protected virtual void TryAlternativeMovement()
        {
            if (alternativeManeuvers.Count > 0)
            {
                MovementStruct maneuver = alternativeManeuvers[0];
                alternativeManeuvers.Remove(alternativeManeuvers[0]);

                if (failedManeuvers.Contains(maneuver))
                {
                    TryAlternativeMovement();
                }
                else
                {
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    GenericMovement newMovementAttempt = Game.Movement.MovementFromStruct(maneuver);

                    if (DebugManager.DebugAI) Debug.Log("Tries: " + newMovementAttempt);

                    newMovementAttempt.Initialize();
                    movementPrediction = new MovementPrediction(newMovementAttempt, CheckSwerveAlternativePrediction);
                }
            }
            else
            {
                if (DebugManager.DebugAI) Messages.ShowInfo("AI doesn't see alternatives to the asteroid collision");
                if (DebugManager.DebugAI) Debug.Log("So AI decides to left it as is...");
                Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
            }
        }

        protected virtual void CheckSwerveAlternativePrediction()
        {
            if ((movementPrediction.AsteroidsHit.Count == 0) && (!movementPrediction.IsOffTheBoard))
            {
                if (DebugManager.DebugAI) Debug.Log("And it works!");
                if (DebugManager.DebugAI) Messages.ShowInfo("AI avoids asteroid collision");

                alternativeManeuvers = new List<MovementStruct>();

                GenericMovement assignedManeuver = movementPrediction.CurrentMovement;
                assignedManeuver.movementPrediction = movementPrediction;

                Selection.ThisShip.SetAssignedManeuver(assignedManeuver);
                Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
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

            result.UpdateColorComplexity();
            return result;
        }

    }
}

