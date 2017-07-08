using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {

        public Vector3 StartingPosition { get; private set; }

        public Movement.GenericMovement AssignedManeuver { get; set; }

        public List<Collider> ObstaclesLanded = new List<Collider>();
        public List<Collider> ObstaclesHit = new List<Collider>();

        public GenericShip LastShipCollision { get; set; }

        public Dictionary<string, Movement.ManeuverColor> Maneuvers { get; private set; }
        public GenericAiTable HotacManeuverTable { get; protected set; }

        // EVENTS

        public event EventHandlerShipMovement AfterGetManeuverColorDecreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverColorIncreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;

        public event EventHandlerShip OnLandedOnObstacle;

        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementExecuted;
        public event EventHandlerShip OnMovementFinish;

        public event EventHandlerShip OnPositionFinish;

        public event EventHandlerShip OnMovementFinishWithColliding;
        public event EventHandlerShip OnMovementFinishWithoutColliding;

        // TRIGGERS

        public void StartMoving()
        {
            if (OnMovementStart != null) OnMovementStart(this);
        }

        public IEnumerator ExecuteMoving()
        {
            if (OnMovementExecuted != null) OnMovementExecuted(this);

            yield return Triggers.ResolveAllTriggers(TriggerTypes.OnShipMovementExecuted);
            yield return Phases.WaitForTemporarySubPhasesFinish();
        }

        public IEnumerator FinishMoving()
        {
            if (OnMovementFinish != null) OnMovementFinish(this);

            yield return Triggers.ResolveAllTriggers(TriggerTypes.OnShipMovementFinish);
            yield return Phases.WaitForTemporarySubPhasesFinish();
        }

        public void FinishPosition()
        {
            if (OnPositionFinish != null) OnPositionFinish(this);
        }

        public void FinishMovementWithColliding()
        {
            if (OnMovementFinishWithColliding != null) OnMovementFinishWithColliding(this);
        }

        public void FinishMovementWithoutColliding()
        {
            if (OnMovementFinishWithoutColliding != null) OnMovementFinishWithoutColliding(this);
        }

        // MANEUVERS

        public Movement.ManeuverColor GetColorComplexityOfManeuver(string maneuverString)
        {
            return Maneuvers[maneuverString];
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
                Movement.MovementStruct movement = Game.Movement.ManeuverFromString(maneuverHolder.Key);
                if (AfterGetManeuverColorDecreaseComplexity != null) AfterGetManeuverColorDecreaseComplexity(this, ref movement);
                if (AfterGetManeuverColorIncreaseComplexity != null) AfterGetManeuverColorIncreaseComplexity(this, ref movement);
                if (AfterGetManeuverAvailablity != null) AfterGetManeuverAvailablity(this, ref movement);
                result.Add(maneuverHolder.Key, movement.ColorComplexity);
            }

            return result;
        }

        public void CheckLandedOnObstacle()
        {
            if (ObstaclesLanded.Count > 0)
            {
                foreach (var obstacle in ObstaclesLanded)
                {
                    if (!ObstaclesHit.Contains(obstacle)) ObstaclesHit.Add(obstacle);
                }

                Game.UI.ShowError("Landed on obstacle");
                if (OnLandedOnObstacle != null) OnLandedOnObstacle(this);
            }
        }

    }

}
