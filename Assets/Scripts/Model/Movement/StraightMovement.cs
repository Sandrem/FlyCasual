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
            LaunchShipMovement();
        }

        protected override float SetProgressTarget()
        {
            Vector3 TargetPosition = new Vector3(0, 0, GetMovement1() + Speed * GetMovement1());
            return TargetPosition.z;
        }

        protected override float SetAnimationSpeed()
        {
            return 0.75f;
        }

        public override void UpdateMovementExecution()
        {
            float progressDelta = AnimationSpeed * Time.deltaTime;
            progressDelta = Mathf.Clamp(progressDelta, 0, Mathf.Abs(ProgressTarget - ProgressCurrent));

            //Vector3 progressDirection = (CollisionReverting) ? Vector3.back : Vector3.forward;
                Vector3 progressDirection = Vector3.forward;

            Selection.ThisShip.SetPosition(Vector3.MoveTowards(Selection.ThisShip.GetPosition(), Selection.ThisShip.GetPosition() + Selection.ThisShip.TransformDirection(progressDirection), progressDelta));
            ProgressCurrent += progressDelta;

            //Selection.ThisShip.RotateModelDuringTurn(CurrentMovementData, PreviousMovementData);
            //UpdateRotationFinisher();

            //CheckCollisionsAfterStraight();

            base.UpdateMovementExecution();
        }

    }

}

