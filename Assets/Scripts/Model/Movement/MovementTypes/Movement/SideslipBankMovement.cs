using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class SideslipBankMovement : NonStraightMovement
    {
        private readonly float[] BANK_SCALES = new float[] { 4.6f, 7.4f, 10.4f };

        public SideslipBankMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, MovementComplexity color) : base(speed, direction, bearing, color)
        {

        }

        public override IEnumerator Perform()
        {
            Initialize();

            Rotate90Hidden();

            movementPrediction = new MovementPrediction(TheShip, this);
            yield return movementPrediction.CalculateMovementPredicition();
        }

        private void Rotate90Hidden()
        {
            Selection.ThisShip.Model.transform.position = Vector3.MoveTowards
            (
                Selection.ThisShip.Model.transform.position,
                Selection.ThisShip.Model.transform.position + Selection.ThisShip.Model.transform.forward,
                -BoardTools.Board.BoardIntoWorld(Selection.ThisShip.ShipBase.SHIPSTAND_SIZE_CM / 2f)
            );

            float rotationModifier = (Direction == ManeuverDirection.Left) ? 1f : -1f;
            Selection.ThisShip.Model.transform.eulerAngles += new Vector3(0, rotationModifier * 90, 0);

            Selection.ThisShip.Model.transform.position = Vector3.MoveTowards
            (
                Selection.ThisShip.Model.transform.position,
                Selection.ThisShip.Model.transform.position + Selection.ThisShip.Model.transform.forward,
                BoardTools.Board.BoardIntoWorld(Selection.ThisShip.ShipBase.SHIPSTAND_SIZE_CM / 2f)
            );

            Selection.ThisShip.GetShipAllPartsTransform().eulerAngles += new Vector3(0, -1 * rotationModifier * 90, 0);
        }

        private void RestoreRotate90Hidden()
        {
            Selection.ThisShip.Model.transform.position = Vector3.MoveTowards
            (
                Selection.ThisShip.Model.transform.position,
                Selection.ThisShip.Model.transform.position + Selection.ThisShip.Model.transform.forward,
                -BoardTools.Board.BoardIntoWorld(Selection.ThisShip.ShipBase.SHIPSTAND_SIZE_CM / 2f)
            );

            float rotationModifier = (Direction == ManeuverDirection.Left) ? -1f : 1f;
            Selection.ThisShip.Model.transform.eulerAngles += new Vector3(0, rotationModifier * 90, 0);

            Selection.ThisShip.Model.transform.position = Vector3.MoveTowards
            (
                Selection.ThisShip.Model.transform.position,
                Selection.ThisShip.Model.transform.position + Selection.ThisShip.Model.transform.forward,
                
                BoardTools.Board.BoardIntoWorld(Selection.ThisShip.ShipBase.SHIPSTAND_SIZE_CM / 2f)
            );

            Selection.ThisShip.GetShipAllPartsTransform().eulerAngles += new Vector3(0, -1 * rotationModifier * 90, 0);
        }

        protected override float SetProgressTarget()
        {
            return 45f;
        }

        protected override float SetAnimationSpeed()
        {
            return 2f * 360f / Speed;
        }

        protected override float SetTurningAroundDistance()
        {
            return GetMovement1() * BANK_SCALES[Speed - 1];
        }

        protected override void ManeuverEndRotation(Action callBack)
        {
            RestoreRotate90Hidden();

            callBack();
        }
    }

}

