using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class SideslipTurnMovement : NonStraightMovement
    {
        private readonly float[] TURN_POINTS = new float[] { 2f, 3.6f, 4.85f };

        public SideslipTurnMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, MovementComplexity color) : base(speed, direction, bearing, color)
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
            return 90;
        }

        protected override float SetAnimationSpeed()
        {
            return 4f * 360f / Speed;
        }

        protected override float SetTurningAroundDistance()
        {
            return GetMovement1() * TURN_POINTS[Speed - 1];
        }

        protected override void ManeuverEndRotation(Action callBack)
        {
            RestoreRotate90Hidden();

            callBack();
        }
    }

}

