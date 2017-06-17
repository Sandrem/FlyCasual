using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    //todo: move to movement
    public enum ManeuverColor
    {
        None,
        Green,
        White,
        Red
    }

    public partial class GenericShip
    {

        public Vector3 StartingPosition { get; private set; }

        public Movement AssignedManeuver { get; set; }

        public List<Collider> ObstaclesLanded = new List<Collider>();
        public List<Collider> ObstaclesHit = new List<Collider>();

        public GenericShip LastShipCollision { get; set; }

        public Dictionary<string, ManeuverColor> Maneuvers { get; private set; }
        public GenericAiTable HotacManeuverTable { get; protected set; }

        // EVENTS

        public event EventHandlerShipMovement AfterGetManeuverColorDecreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverColorIncreaseComplexity;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;

        public event EventHandlerShip OnLandedOnObstacle;

        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementFinish;

        public event EventHandlerShip OnPositionFinish;

        public event EventHandlerShip OnMovementFinishWithColliding;
        public event EventHandlerShip OnMovementFinishWithoutColliding;

        // TRIGGERS

        public void StartMoving()
        {
            if (OnMovementStart != null) OnMovementStart(this);
        }

        public IEnumerator FinishMoving()
        {
            if (OnMovementFinish != null) OnMovementFinish(this);

            while (!Triggers.Empty)
            {
                Debug.Log("Call trigger!");
                yield return Triggers.CallTrigger(TriggerTypes.OnShipMovementFinish);
            }

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

        public void FinishMovingWithoutColliding()
        {
            if (OnMovementFinishWithoutColliding != null) OnMovementFinishWithoutColliding(this);
        }

        // MANEUVERS

        public ManeuverColor GetColorComplexityOfManeuver(string maneuverString)
        {
            return Maneuvers[maneuverString];
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
                Movement movement = Game.Movement.ManeuverFromString(maneuverHolder.Key);
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
