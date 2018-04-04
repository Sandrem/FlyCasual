using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using System;

namespace Ship
{

    public partial class GenericShip
    {

        public Vector3 StartingPosition { get; private set; }

        public GenericMovement AssignedManeuver { get; private set; }

        public bool IsIgnoreObstacles;

        public bool IsLandedOnObstacle;

        public bool IsHitObstacles
        {
            get { return !IsIgnoreObstacles && ObstaclesHit.Count != 0; }
        }

        public List<Collider> ObstaclesHit = new List<Collider>();

        public List<GameObject> MinesHit = new List<GameObject>();

        public bool IsBumped
        {
            get { return ShipsBumped.Count != 0; }
        }

        public List<GenericShip> ShipsBumped = new List<GenericShip>();

        public GenericShip LastShipCollision { get; set; }

        public Dictionary<string, ManeuverColor> Maneuvers { get; private set; }
        public AI.GenericAiTable HotacManeuverTable { get; protected set; }

        // EVENTS

        public event EventHandlerShipMovement AfterGetManeuverColorDecreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverColorIncreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;

        public event EventHandlerShip OnManeuverIsReadyToBeRevealed;
        public event EventHandlerShip OnManeuverIsRevealed;
        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementExecuted;
        public event EventHandlerShip OnMovementFinish;
        public static event EventHandlerShip OnMovementFinishGlobal;

        public event EventHandlerShip OnPositionFinish;
        public static event EventHandler OnPositionFinishGlobal;

        // TRIGGERS

        public void CallManeuverIsReadyToBeRevealed(System.Action callBack)
        {
            if (OnManeuverIsReadyToBeRevealed != null) OnManeuverIsReadyToBeRevealed(this);

            Triggers.ResolveTriggers (TriggerTypes.OnManeuverIsReadyToBeRevealed, callBack);
        }

        public void CallManeuverIsRevealed(System.Action callBack)
        {
            Roster.ToggelManeuverVisibility(Selection.ThisShip, true);

            if (Selection.ThisShip.AssignedManeuver.IsRealMovement)
            {
                if (OnManeuverIsRevealed != null) OnManeuverIsRevealed(this);

                Triggers.ResolveTriggers(TriggerTypes.OnManeuverIsRevealed, callBack);
            }
            else // For ionized ships
            {
                callBack();
            }
        }

        public void StartMoving(System.Action callback)
        {
            if (OnMovementStart != null) OnMovementStart(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipMovementStart, callback);
        }

        public void CallExecuteMoving(Action callback)
        {
            if (OnMovementExecuted != null) OnMovementExecuted(this);

            Triggers.ResolveTriggers(
                TriggerTypes.OnShipMovementExecuted,
                delegate { Selection.ThisShip.CallFinishMovement(callback); }
            );
        }

        public void CallFinishMovement(Action callback)
        {
            if (OnMovementFinish != null) OnMovementFinish(this);
            if (OnMovementFinishGlobal != null) OnMovementFinishGlobal(this);

            Triggers.ResolveTriggers(
                TriggerTypes.OnShipMovementFinish,
                delegate () {
                    Roster.HideAssignedManeuverDial(this);
                    Selection.ThisShip.FinishPosition(callback);
                });
        }

        public void FinishPosition(System.Action callback)
        {
            if (OnPositionFinish != null) OnPositionFinish(this);
            if (OnPositionFinishGlobal != null) OnPositionFinishGlobal();

            Triggers.ResolveTriggers(TriggerTypes.OnPositionFinish, callback);
        }

        // MANEUVERS

        // TODO: Rewrite
        public ManeuverColor GetColorComplexityOfManeuver(MovementStruct movement)
        {
            if (AfterGetManeuverColorDecreaseComplexity != null) AfterGetManeuverColorDecreaseComplexity(this, ref movement);
            if (AfterGetManeuverColorIncreaseComplexity != null) AfterGetManeuverColorIncreaseComplexity(this, ref movement);
            if (AfterGetManeuverAvailablity != null) AfterGetManeuverAvailablity(this, ref movement);

            return movement.ColorComplexity;
        }

        public ManeuverColor GetLastManeuverColor()
        {
            ManeuverColor result = ManeuverColor.None;

            result = AssignedManeuver.ColorComplexity;
            return result;
        }

        public Dictionary<string, ManeuverColor> GetManeuvers()
        {
            Dictionary<string, ManeuverColor> result = new Dictionary<string, ManeuverColor>();

            foreach (var maneuverHolder in Maneuvers)
            {
                result.Add(maneuverHolder.Key, new MovementStruct(maneuverHolder.Key).ColorComplexity);
            }

            return result;
        }

        public bool HasManeuver(string maneuverString)
        {
            bool result = false;
            if (Maneuvers.ContainsKey(maneuverString))
            {
                result = (Maneuvers[maneuverString] != ManeuverColor.None);
            }
            return result;
        }

        public bool HasManeuver(MovementStruct maneuverStruct)
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
    }

}