using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public abstract class NonStraightMovement : GenericMovement
    {
        protected float TurningAroundDistance;
        protected bool MovementFinisherLaunched;

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
            if (!MovementFinisherLaunched)
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
            }
            else
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
            }


            base.UpdateMovementExecution();
        }

        protected override void CheckFinish()
        {
            if (ProgressCurrent == ProgressTarget)
            {
                if (MovementFinisherLaunched)
                {
                    Selection.ThisShip.FinishMovementWithoutColliding();

                    MovementTemplates.HideLastMovementRuler();

                    //TEMP
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Movement.isMoving = false;

                    Game.StartCoroutine(FinishMovementCoroutine());
                }
                else
                {
                    LaunchMovementFinisher();
                }
            }
        }

        protected virtual void LaunchMovementFinisher()
        {
            ProgressCurrent = 0;

            Vector3 TargetPosition = new Vector3(0, 0, GetMovement1());
            ProgressTarget = TargetPosition.z;

            AnimationSpeed = 0.75f;

            MovementFinisherLaunched = true;
        }

    }

}

