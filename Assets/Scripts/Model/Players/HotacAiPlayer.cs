using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class HotacAiPlayer : GenericAiPlayer
    {
        private bool inDebug = false;

        private Movement.MovementPrediction movementPrediction;
        private List<Movement.MovementStruct> alternativeManeuvers;

        public HotacAiPlayer(): base() {
            Name = "HotAC AI";
        }

        public override void ActivateShip(Ship.GenericShip ship)
        {
            if (inDebug) Debug.Log("=== " + ship.PilotName + " (" + ship.ShipId + ") ===");

            Ship.GenericShip anotherShip = FindNearestEnemyShip(ship, ignoreCollided:true, inArcAndRange:true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship, ignoreCollided: true);
            if (anotherShip == null) anotherShip = FindNearestEnemyShip(ship);
            if (inDebug) Debug.Log("Nearest enemy is " + anotherShip.PilotName + " (" + anotherShip.ShipId + ")");

            ship.GenerateAvailableActionsList();
            foreach (var action in ship.GetAvailableActionsList())
            {
                if (action.GetType() == typeof(ActionsList.TargetLockAction))
                {
                    Actions.AssignTargetLockToPair(ship, anotherShip);
                    break;
                }
            }

            ship.AssignedManeuver = ship.HotacManeuverTable.GetManeuver(ship, anotherShip);
            PerformManeuverOfShip(ship);
        }

        public override void PerformAction()
        {
            bool actionIsPerformed = false;

            if (Selection.ThisShip.GetToken(typeof(Tokens.StressToken)) != null)
            {
                Selection.ThisShip.RemoveToken(typeof(Tokens.StressToken));
            }
            else if (Selection.ThisShip.GetAvailableActionsList().Count > 0)
            {
                actionIsPerformed = TryToCancelCrits();
                if (!actionIsPerformed) actionIsPerformed = TryToGetShot();
                if (!actionIsPerformed) actionIsPerformed = TryFocus();
                if (!actionIsPerformed) actionIsPerformed = TryEvade();
            }

            if (!actionIsPerformed)
            {
                Phases.CurrentSubPhase.CallBack();
            }
        }

        private bool TryToCancelCrits()
        {
            return false;
        }

        private bool TryToGetShot()
        {
            return false;
        }

        private bool TryToAvoidShot()
        {
            return false;
        }

        private bool TryFocus()
        {
            if (Actions.HasTarget(Selection.ThisShip))
            {
                foreach (var availableAction in Selection.ThisShip.GetAvailableActionsList())
                {
                    if (availableAction.GetType() == typeof(ActionsList.FocusAction))
                    {
                        availableAction.ActionTake();
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TryEvade()
        {
            foreach (var availableAction in Selection.ThisShip.GetAvailableActionsList())
            {
                if (availableAction.GetType() == typeof(ActionsList.EvadeAction))
                {
                    availableAction.ActionTake();
                    return true;
                }
            }
            return false;
        }

        //TODO: move to a sub-class
        public void Swerve()
        {
            alternativeManeuvers = GetAlternativeManeuvers(Selection.ThisShip.AssignedManeuver);
            TryAlternativeMovement();
        }

        private void TryAlternativeMovement()
        {
            Movement.GenericMovement newMovementAttempt = Game.Movement.MovementFromStruct(alternativeManeuvers[0]);
            alternativeManeuvers.Remove(alternativeManeuvers[0]);

            if (DebugManager.DebugAI) Debug.Log("Tries: " + newMovementAttempt);

            newMovementAttempt.Initialize();
            movementPrediction = new Movement.MovementPrediction(newMovementAttempt, CheckSwerveAlternativePrediction);
        }

        private void CheckSwerveAlternativePrediction()
        {
            if (movementPrediction.AsteroidsHit.Count == 0)
            {
                if (DebugManager.DebugAI) Debug.Log("And it works!");
                Messages.ShowInfo("AI avoids asteroid collision");

                alternativeManeuvers = new List<Movement.MovementStruct>();

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

        private List<Movement.MovementStruct> GetAlternativeManeuvers(Movement.GenericMovement maneuver)
        {
            List<Movement.MovementStruct> alternativeManeuvers = new List<Movement.MovementStruct>();

            Movement.MovementStruct movementStruct = new Movement.MovementStruct
            {
                Speed = maneuver.ManeuverSpeed,
                Bearing = maneuver.Bearing,
                Direction = maneuver.Direction,
                ColorComplexity = maneuver.ColorComplexity
            };

            Movement.MovementStruct alternativeMovementStruct = movementStruct;
            //TODO: use struct instead of string
            switch (maneuver.Bearing)
            {
                case Movement.ManeuverBearing.Straight:
                    alternativeMovementStruct.Bearing = Movement.ManeuverBearing.Bank;

                    alternativeMovementStruct.Direction = Movement.ManeuverDirection.Left;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));

                    alternativeMovementStruct.Direction = Movement.ManeuverDirection.Right;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));
                    break;
                case Movement.ManeuverBearing.Bank:
                    alternativeMovementStruct.Bearing = Movement.ManeuverBearing.Turn;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));

                    alternativeMovementStruct.Bearing = Movement.ManeuverBearing.Straight;
                    alternativeMovementStruct.Direction = Movement.ManeuverDirection.Forward;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));
                    break;
                case Movement.ManeuverBearing.Turn:
                    alternativeMovementStruct.Bearing = Movement.ManeuverBearing.Bank;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));
                    break;
                case Movement.ManeuverBearing.KoiogranTurn:
                    alternativeMovementStruct.Bearing = Movement.ManeuverBearing.Bank;

                    alternativeMovementStruct.Direction = Movement.ManeuverDirection.Left;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));

                    alternativeMovementStruct.Direction = Movement.ManeuverDirection.Right;
                    alternativeManeuvers.Add(GetSimilarManeuverByStruct(alternativeMovementStruct));
                    break;
                default:
                    break;
            }

            return alternativeManeuvers;
        }

        private Movement.MovementStruct GetSimilarManeuverByStruct(Movement.MovementStruct alternativeManeuverStruct)
        {
            Movement.MovementStruct result = alternativeManeuverStruct;

            if (!Selection.ThisShip.HasManeuver(result))
            {
                if (result.Speed == Movement.ManeuverSpeed.Speed1)
                {
                    result.Speed = Movement.ManeuverSpeed.Speed2;
                }
                else if ((result.Speed == Movement.ManeuverSpeed.Speed4) || (result.Speed == Movement.ManeuverSpeed.Speed5))
                {
                    result.Speed = Movement.ManeuverSpeed.Speed4;
                    if (!Selection.ThisShip.HasManeuver(result))
                    {
                        result.Speed = Movement.ManeuverSpeed.Speed3;
                        if (!Selection.ThisShip.HasManeuver(result))
                        {
                            result.Speed = Movement.ManeuverSpeed.Speed2;
                        }
                    }
                }
            }

            return result;
        }

    }

}
