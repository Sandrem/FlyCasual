using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class ReverseBankMovement : NonStraightMovement
    {
        private readonly float[] BANK_SCALES = new float[] { 4.6f, 7.4f, 10.4f };

        public ReverseBankMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, MovementComplexity color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            base.Perform();

            Rotate180Hidden();
            AlterDirection();

            movementPrediction = new MovementPrediction(this, TheShip.Owner.AfterShipMovementPrediction);
        }

        private void Rotate180Hidden()
        {
            Selection.ThisShip.Model.transform.eulerAngles += new Vector3(0, 180, 0);
            Selection.ThisShip.Model.transform.position = Vector3.MoveTowards(
                Selection.ThisShip.Model.transform.position,
                Selection.ThisShip.Model.transform.position + Selection.ThisShip.Model.transform.forward,
                BoardTools.Board.BoardIntoWorld(Selection.ThisShip.ShipBase.SHIPSTAND_SIZE_CM)
            );
            Selection.ThisShip.GetShipAllPartsTransform().eulerAngles += new Vector3(0, 180, 0);
        }

        private void AlterDirection()
        {
            Direction = (Direction == ManeuverDirection.Left) ? ManeuverDirection.Right : ManeuverDirection.Left;
        }

        protected override float SetProgressTarget()
        {
            return 45f;
        }

        protected override float SetAnimationSpeed()
        {
            return 360f / Speed;
        }

        protected override float SetTurningAroundDistance()
        {
            return GetMovement1() * BANK_SCALES[Speed - 1];
        }

        protected override void ManeuverEndRotation(Action callBack)
        {
            Rotate180Hidden();
            AlterDirection();

            callBack();
        }
    }

}

