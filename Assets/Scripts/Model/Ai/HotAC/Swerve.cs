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

        protected GameManagerScript Game;

        public Swerve()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            alternativeManeuvers = GetAlternativeManeuvers(Selection.ThisShip.AssignedManeuver);
            TryAlternativeMovement();
        }

        protected void TryAlternativeMovement()
        {
            GenericMovement newMovementAttempt = Game.Movement.MovementFromStruct(alternativeManeuvers[0]);
            alternativeManeuvers.Remove(alternativeManeuvers[0]);

            if (DebugManager.DebugAI) Debug.Log("Tries: " + newMovementAttempt);

            newMovementAttempt.Initialize();
            movementPrediction = new MovementPrediction(newMovementAttempt, CheckSwerveAlternativePrediction);
        }

        protected void CheckSwerveAlternativePrediction()
        {
            if ((movementPrediction.AsteroidsHit.Count == 0) && (!movementPrediction.IsOffTheBoard))
            {
                if (DebugManager.DebugAI) Debug.Log("And it works!");
                Messages.ShowInfo("AI avoids asteroid collision");

                alternativeManeuvers = new List<MovementStruct>();

                Selection.ThisShip.AssignedManeuver = movementPrediction.CurrentMovement;
                Selection.ThisShip.AssignedManeuver.movementPrediction = movementPrediction;
                Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
            }
            else
            {
                if (alternativeManeuvers.Count > 0)
                {
                    if (DebugManager.DebugAI) Debug.Log("But it doesn't works...");
                    TryAlternativeMovement();
                }
                else
                {
                    Messages.ShowInfo("AI doesn't see alternatives to the asteroid collision");
                    if (DebugManager.DebugAI) Debug.Log("So AI decides to left it as is...");
                    Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
                }

            }
        }

        protected List<MovementStruct> GetAlternativeManeuvers(GenericMovement maneuver)
        {
            List<MovementStruct> alternativeManeuvers = new List<MovementStruct>();

            MovementStruct movementStruct = new MovementStruct
            {
                Speed = maneuver.ManeuverSpeed,
                Bearing = maneuver.Bearing,
                Direction = maneuver.Direction,
                ColorComplexity = maneuver.ColorComplexity
            };

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

            if (!Selection.ThisShip.HasManeuver(result))
            {
                if (result.Speed == ManeuverSpeed.Speed1)
                {
                    result.SpeedInt++;
                }
                else
                {
                    result.SpeedInt--;
                    if (!Selection.ThisShip.HasManeuver(result))
                    {
                        result.SpeedInt--;
                        if (!Selection.ThisShip.HasManeuver(result))
                        {
                            result.SpeedInt--;  //for example, 5 bank -> 2 bank
                        }
                    }
                }
            }

            return result;
        }

    }
}

