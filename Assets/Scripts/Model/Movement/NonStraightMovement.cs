using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public abstract class NonStraightMovement : GenericMovement
    {
        protected float TurningAroundDistance { get; set; }

        public NonStraightMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            base.Perform();
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
            TurningAroundDistance = SetTurningAroundDistance();
        }

        protected virtual float SetTurningAroundDistance()
        {
            return 0;
        }

        public override void UpdateMovementExecution()
        {
            float progressDelta = AnimationSpeed * Time.deltaTime;
            progressDelta = Mathf.Clamp(progressDelta, 0, Mathf.Abs(ProgressTarget - ProgressCurrent));

            float turningDirection = (Direction == ManeuverDirection.Right) ? 1 : -1;
            //int progressDirection = (CollisionReverting) ? -1 : 1;
                int progressDirection = 1;

            Selection.ThisShip.Rotate(Selection.ThisShip.TransformPoint(new Vector3(TurningAroundDistance * turningDirection, 0, 0)), turningDirection * progressDelta * progressDirection);
            ProgressCurrent += progressDelta;

            //Selection.ThisShip.RotateModelDuringTurn(CurrentMovementData, PreviousMovementData);
            //UpdateRotation();

            //CheckCollisionsAfterNonStraight();

            base.UpdateMovementExecution();
        }
    }

}

