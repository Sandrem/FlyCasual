using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class StraightMovement : GenericMovement
    {
        public StraightMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            base.Perform();
            Initialize();

            PlanMovement();
            LaunchShipMovement();
        }

        protected override float SetProgressTarget()
        {
            return GetMovement1() + Speed * GetMovement1();
        }

        protected override float SetAnimationSpeed()
        {
            return 0.75f;
        }

        public override void UpdateMovementExecution()
        {
            float progressDelta = AnimationSpeed * Time.deltaTime;
            progressDelta = Mathf.Clamp(progressDelta, 0, Mathf.Abs(ProgressTarget - ProgressCurrent));
            ProgressCurrent += progressDelta;

            Selection.ThisShip.SetPosition(Vector3.MoveTowards(Selection.ThisShip.GetPosition(), Selection.ThisShip.GetPosition() + Selection.ThisShip.TransformDirection(Vector3.forward), progressDelta));

            //UpdateRotationFinisher();

            base.UpdateMovementExecution();
        }

        protected override void PlanMovement()
        {
            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            float distancePart = (GetMovement1() + Speed * GetMovement1())/10;
            Vector3 position = Selection.ThisShip.GetPosition();

            for (int i = 1; i <= 10; i++)
            {
                position = Vector3.MoveTowards(position, position + Selection.ThisShip.TransformDirection(Vector3.forward), distancePart);
                GameObject ShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, position, Selection.ThisShip.GetRotation(), Board.GetBoard());
            }
        }

    }

}

