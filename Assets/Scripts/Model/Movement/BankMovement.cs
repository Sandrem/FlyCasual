using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class BankMovement : NonStraightMovement
    {
        private readonly float[] BANK_SCALES = new float[] { 4.6f, 7.4f, 10.4f };

        public BankMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            base.Perform();

            movementPrediction = new MovementPrediction(this);
        }

        protected override float SetProgressTarget()
        {
            return 45f;
        }

        protected override float SetAnimationSpeed()
        {
            return 60f / Speed;
        }

        protected override float SetTurningAroundDistance()
        {
            return GetMovement1() * BANK_SCALES[Speed - 1];
        }
    }

}

