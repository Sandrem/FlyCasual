using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class SegnorsLoopMovement : NonStraightMovement
    {
        private readonly float[] BANK_SCALES = new float[] { 4.6f, 7.4f, 10.4f };

        public SegnorsLoopMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            base.Perform();

            movementPrediction = new MovementPrediction(this, Selection.ThisShip.Owner.AfterShipMovementPrediction);
        }

        protected override float SetProgressTarget()
        {
            return 45f;
        }

        protected override float SetAnimationSpeed()
        {
            return 240f / Speed;
        }

        protected override float SetTurningAroundDistance()
        {
            return GetMovement1() * BANK_SCALES[Speed - 1];
        }

        protected override void ManeuverEndRotation(Action callBack)
        {
            if (!Selection.ThisShip.IsBumped)
            {
                Phases.StartTemporarySubPhaseOld("Segnor's Loop", typeof(SubPhases.KoiogranTurnSubPhase), callBack);
            }
            else
            {
                //todo: Error about failed koiogran turn
                //Messages.ShowError("Koiogran Turn is failed due to collision");
                callBack();
            }
        }
    }

}

