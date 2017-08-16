using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class TurnMovement : NonStraightMovement
    {
        private readonly float[] TURN_POINTS = new float[] { 2f, 3.6f, 4.85f };

        public TurnMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            base.Perform();

            movementPrediction = new MovementPrediction(this, CheckShipMovementPrediction);        
        }

        protected override float SetProgressTarget()
        {
            return 90f;
        }

        protected override float SetAnimationSpeed()
        {
            return 100f / Speed;
        }

        protected override float SetTurningAroundDistance()
        {
            return GetMovement1() * TURN_POINTS[Speed - 1];
        }
    }

}

