using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {

        public Vector3 StartingPosition { get; private set; }

        public Movement.GenericMovement AssignedManeuver { get; set; }

        public bool IsLandedOnObstacle;
        public List<Collider> ObstaclesHit = new List<Collider>();

        public bool IsBumped
        {
            get { return ShipsBumped.Count != 0; }
        }

        public List<GenericShip> ShipsBumped = new List<GenericShip>();

        public GenericShip LastShipCollision { get; set; }

        public Dictionary<string, Movement.ManeuverColor> Maneuvers { get; private set; }
        public AI.GenericAiTable HotacManeuverTable { get; protected set; }

        // EVENTS

        public event EventHandlerShipMovement AfterGetManeuverColorDecreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverColorIncreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;

        public event EventHandlerShip OnManeuverIsReadyToBeRevealed;
        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementExecuted;
        public event EventHandlerShip OnMovementFinish;

        public event EventHandlerShip OnPositionFinish;
        public static event EventHandler OnPositionFinishGlobal;

        // TRIGGERS

        public void CallManeuverIsReadyToBeRevealed()
        {
            if (OnManeuverIsReadyToBeRevealed != null) OnManeuverIsReadyToBeRevealed(this);
        }

        public void StartMoving(System.Action callback)
        {
            if (OnMovementStart != null) OnMovementStart(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipMovementStart, callback);
        }

        public void CallExecuteMoving()
        {
            if (OnMovementExecuted != null) OnMovementExecuted(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipMovementExecuted, delegate() { Selection.ThisShip.CallFinishMovement(); });
        }

        public void CallFinishMovement()
        {
            if (OnMovementFinish != null) OnMovementFinish(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipMovementFinish, delegate () { Selection.ThisShip.FinishPosition(delegate () { Phases.FinishSubPhase(typeof(SubPhases.MovementExecutionSubPhase)); }); });
        }

        public void FinishPosition(System.Action callback)
        {
            if (OnPositionFinish != null) OnPositionFinish(this);
            if (OnPositionFinishGlobal != null) OnPositionFinishGlobal();

            Triggers.ResolveTriggers(TriggerTypes.OnPositionFinish, callback);
        }

        // MANEUVERS

        // TODO: Rewrite
        public Movement.ManeuverColor GetColorComplexityOfManeuver(Movement.MovementStruct movement)
        {
            if (AfterGetManeuverColorDecreaseComplexity != null) AfterGetManeuverColorDecreaseComplexity(this, ref movement);
            if (AfterGetManeuverColorIncreaseComplexity != null) AfterGetManeuverColorIncreaseComplexity(this, ref movement);
            if (AfterGetManeuverAvailablity != null) AfterGetManeuverAvailablity(this, ref movement);

            return movement.ColorComplexity;
        }

        public Movement.ManeuverColor GetLastManeuverColor()
        {
            Movement.ManeuverColor result = Movement.ManeuverColor.None;

            result = AssignedManeuver.ColorComplexity;
            return result;
        }

        public Dictionary<string, Movement.ManeuverColor> GetManeuvers()
        {
            Dictionary<string, Movement.ManeuverColor> result = new Dictionary<string, Movement.ManeuverColor>();

            foreach (var maneuverHolder in Maneuvers)
            {
                result.Add(maneuverHolder.Key, new Movement.MovementStruct(maneuverHolder.Key).ColorComplexity);
            }

            return result;
        }

        public bool HasManeuver(string maneuverString)
        {
            bool result = false;
            if (Maneuvers.ContainsKey(maneuverString))
            {
                result = (Maneuvers[maneuverString] != Movement.ManeuverColor.None);
            }
            return result;
        }

        public bool HasManeuver(Movement.MovementStruct maneuverStruct)
        {
            string maneuverString = maneuverStruct.ToString();
            return HasManeuver(maneuverString);
        }

    }

}
