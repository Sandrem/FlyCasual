using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using System;
using Obstacles;
using System.Linq;

namespace Ship
{

    public partial class GenericShip
    {

        public Vector3 StartingPosition { get; private set; }

        public GenericMovement AssignedManeuver { get; private set; }

        public bool IsIgnoreObstacles;
        public bool IsIgnoreObstaclesDuringBoost;
        public bool IsIgnoreObstaclesDuringBarrelRoll;
        public bool IsIgnoreObstacleObstructionWhenAttacking;

        public List<GenericObstacle> IgnoreObstaclesList = new List<GenericObstacle>();
        public List<Type> IgnoreObstacleTypes = new List<Type>();

        public bool IsLandedOnObstacle
        {
            get
            {
                return LandedOnObstacles.Any(o => !IgnoreObstaclesList.Contains(o));
            }

            set
            {
                if (value == false) LandedOnObstacles = new List<GenericObstacle>();
            }
        }

        public List<GenericObstacle> LandedOnObstacles = new List<GenericObstacle>();

        public bool IsHitObstacles
        {
            get
            {
                return !IsIgnoreObstacles && ObstaclesHit.Any(o => !IgnoreObstacleTypes.Contains(o.GetType())) && ObstaclesHit.Any(o => !IgnoreObstaclesList.Contains(o));
            }

            set
            {
                if (value == false) ObstaclesHit = new List<GenericObstacle>();
            }
        }

        public List<GenericObstacle> ObstaclesHit = new List<GenericObstacle>();

        public List<GameObject> MinesHit = new List<GameObject>();

        public bool IsBumped
        {
            get { return ShipsBumped.Count != 0; }
        }

        public List<GenericShip> ShipsBumped = new List<GenericShip>();

        public GenericShip LastShipCollision { get; set; }

        public Dictionary<string, MovementComplexity> Maneuvers { get; set; }
        public AI.GenericAiTable HotacManeuverTable { get; protected set; }

        // EVENTS

        public event EventHandlerShipMovement AfterGetManeuverColorDecreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverColorIncreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;

        public event EventHandlerShip OnManeuverIsReadyToBeRevealed;
        public static event EventHandlerShip OnManeuverIsReadyToBeRevealedGlobal;
        public event EventHandlerShip OnManeuverIsRevealed;
        public static event EventHandlerShip OnManeuverIsRevealedGlobal;
        public static event EventHandlerShip OnNoManeuverWasRevealedGlobal;
        public event EventHandlerShip BeforeMovementIsExecuted;
        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementExecuted;
        public event EventHandlerShip OnMovementFinish;
        public event EventHandlerShip OnMovementFinishSuccessfully;
        public event EventHandlerShip OnMovementFinishUnsuccessfully;
        public event EventHandlerShip OnMovementBumped;
        public static event EventHandlerShip OnMovementFinishGlobal;

        public event EventHandlerShip OnPositionFinish;
        public static event EventHandlerShip OnPositionFinishGlobal;

        // TRIGGERS

        public void CallManeuverIsReadyToBeRevealed(System.Action callBack)
        {
            if (Selection.ThisShip.AssignedManeuver != null && Selection.ThisShip.AssignedManeuver.IsRevealDial)
            {
                if (OnManeuverIsReadyToBeRevealedGlobal != null) OnManeuverIsReadyToBeRevealedGlobal(this);
                if (OnManeuverIsReadyToBeRevealed != null) OnManeuverIsReadyToBeRevealed(this);

                Triggers.ResolveTriggers(TriggerTypes.OnManeuverIsReadyToBeRevealed, callBack);
            }
            else  // For ionized ships
            {
                callBack();
            }
        }

        public void CallManeuverIsRevealed(System.Action callBack)
        {
            if (Selection.ThisShip.AssignedManeuver != null) Roster.ToggleManeuverVisibility(Selection.ThisShip, true);

            if (Selection.ThisShip.AssignedManeuver != null && Selection.ThisShip.AssignedManeuver.IsRevealDial)
            {
                if (OnManeuverIsRevealed != null) OnManeuverIsRevealed(this);
                if (OnManeuverIsRevealedGlobal != null) OnManeuverIsRevealedGlobal(this);

                Triggers.ResolveTriggers(TriggerTypes.OnManeuverIsRevealed, callBack);
            }
            else // For ionized ships
            {
                if (OnNoManeuverWasRevealedGlobal != null) OnNoManeuverWasRevealedGlobal(this);

                callBack();
            }
        }

        public void StartMoving(System.Action callback)
        {
            if (OnMovementStart != null) OnMovementStart(this);

            Triggers.ResolveTriggers(TriggerTypes.OnMovementStart, callback);
        }


        public void CallExecuteMoving(Action callback)
        {
            if (OnMovementExecuted != null) OnMovementExecuted(this);

            Triggers.ResolveTriggers(
                TriggerTypes.OnMovementExecuted,
                delegate { Selection.ThisShip.CallFinishMovement(callback); }
            );
        }

        public void CallBeforeMovementIsExecuted(Action callback)
        {
            if (BeforeMovementIsExecuted != null) BeforeMovementIsExecuted(this);

            Triggers.ResolveTriggers(
                TriggerTypes.BeforeMovementIsExecuted,
                callback
            );
        }

        public void CallOnMovementBumped(GenericShip ship)
        {
            if (OnMovementBumped != null) OnMovementBumped(ship);
        }

        public void CallFinishMovement(Action callback)
        {
            if (OnMovementFinish != null) OnMovementFinish(this);
            if (OnMovementFinishGlobal != null) OnMovementFinishGlobal(this);
            
            // If we didn't bump, or end up off the board then we have succesfully completed our manuever.
            if (!IsBumped && !BoardTools.Board.IsOffTheBoard(this))
            {
                if (OnMovementFinishSuccessfully != null) OnMovementFinishSuccessfully(this);
            }
            else if(IsBumped)
            {
                if (OnMovementFinishUnsuccessfully != null) OnMovementFinishUnsuccessfully(this);

                foreach(GenericShip ship in ShipsBumped)
                {
                    ship.CallOnMovementBumped(this);
                }
            }

            Triggers.ResolveTriggers(
                TriggerTypes.OnMovementFinish,
                delegate () {
                    Roster.HideAssignedManeuverDial(this);
                    Selection.ThisShip.FinishPosition(callback);
                });
        }

        public void FinishPosition(System.Action callback)
        {
            if (OnPositionFinish != null) OnPositionFinish(this);
            if (OnPositionFinishGlobal != null) OnPositionFinishGlobal(this);

            Triggers.ResolveTriggers(TriggerTypes.OnPositionFinish, callback);
        }

        // MANEUVERS

        // TODO: Rewrite
        public MovementComplexity GetColorComplexityOfManeuver(ManeuverHolder movement)
        {
            if (AfterGetManeuverColorDecreaseComplexity != null) AfterGetManeuverColorDecreaseComplexity(this, ref movement);
            if (AfterGetManeuverColorIncreaseComplexity != null) AfterGetManeuverColorIncreaseComplexity(this, ref movement);
            if (AfterGetManeuverAvailablity != null) AfterGetManeuverAvailablity(this, ref movement);

            return movement.ColorComplexity;
        }

        public MovementComplexity GetLastManeuverColor()
        {
            MovementComplexity result = MovementComplexity.None;

            result = AssignedManeuver.ColorComplexity;
            return result;
        }

        public ManeuverBearing GetLastManeuverBearing()
        {               
            var result = AssignedManeuver.Bearing;
            return result;
        }

        public Dictionary<string, MovementComplexity> GetManeuvers()
        {
            Dictionary<string, MovementComplexity> result = new Dictionary<string, MovementComplexity>();

            foreach (var maneuverHolder in Maneuvers)
            {
                result.Add(maneuverHolder.Key, new ManeuverHolder(maneuverHolder.Key).ColorComplexity);
            }

            return result;
        }

        public List<ManeuverHolder> GetManeuverHolders()
        {
            List<ManeuverHolder> maneuverHolders = new List<ManeuverHolder>();

            foreach (var maneuverHolder in Maneuvers)
            {
                maneuverHolders.Add(new ManeuverHolder(maneuverHolder.Key, this));
            }

            return maneuverHolders;
        }

        public bool HasManeuver(string maneuverString)
        {
            bool result = false;
            if (Maneuvers.ContainsKey(maneuverString))
            {
                result = (Maneuvers[maneuverString] != MovementComplexity.None);
            }
            return result;
        }

        public bool HasManeuver(ManeuverHolder maneuverStruct)
        {
            string maneuverString = maneuverStruct.ToString();
            return HasManeuver(maneuverString);
        }

        public void SetAssignedManeuver(GenericMovement movement)
        {
            AssignedManeuver = movement;
            Roster.UpdateAssignedManeuverDial(this, movement);
        }

        public void ClearAssignedManeuver()
        {
            AssignedManeuver = null;
        }

        public void Rotate180(Action callBack)
        {
            Phases.StartTemporarySubPhaseOld("Rotate ship 180°", typeof(SubPhases.KoiogranTurnSubPhase), callBack);
        }

        public void Rotate90Clockwise(Action callBack)
        {
            Phases.StartTemporarySubPhaseOld("Rotate ship 90°", typeof(SubPhases.Rotate90ClockwiseSubPhase), callBack);
        }

        public void Rotate90Counterclockwise(Action callBack)
        {
            Phases.StartTemporarySubPhaseOld("Rotate ship -90°", typeof(SubPhases.Rotate90CounterclockwiseSubPhase), callBack);
        }
    }

}