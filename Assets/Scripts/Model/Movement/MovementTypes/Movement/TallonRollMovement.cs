using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class TallonRollMovement : NonStraightMovement
    {
        private readonly float[] TURN_POINTS = new float[] { 2f, 3.6f, 4.85f };

        public TallonRollMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            base.Perform();

            movementPrediction = new MovementPrediction(this, TheShip.Owner.AfterShipMovementPrediction);
        }

        protected override float SetProgressTarget()
        {
            return 90f;
        }

        protected override float SetAnimationSpeed()
        {
            return 720f / Speed;
        }

        protected override float SetTurningAroundDistance()
        {
            return GetMovement1() * TURN_POINTS[Speed - 1];
        }

        protected override void ManeuverEndRotation(Action callBack)
        {
            if (!TheShip.IsBumped)
            {
                Phases.StartTemporarySubPhaseOld("Tallon Roll", typeof(SubPhases.TallonRollSubPhase), callBack);
            }
            else
            {
                //todo: Error about failed Tallon Roll
                //Messages.ShowError("Tallon Roll is failed due to collision");
                callBack();
            }
        }
    }

}

