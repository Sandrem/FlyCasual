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
            float progressDelta = AnimationSpeed * Time.deltaTime;
            progressDelta = Mathf.Clamp(progressDelta, 0, Mathf.Abs(ProgressTarget - ProgressCurrent));
            ProgressCurrent += progressDelta;

            if (!MovementFinisherLaunched)
            {
                float turningDirection = (Direction == ManeuverDirection.Right) ? 1 : -1;

                int progressDirection = 1;
                Selection.ThisShip.RotateAround(Selection.ThisShip.TransformPoint(new Vector3(TurningAroundDistance * turningDirection, 0, 0)), turningDirection * progressDelta * progressDirection);

                //Selection.ThisShip.RotateModelDuringTurn(CurrentMovementData, PreviousMovementData);
                UpdateRotation();
            }
            else
            {
                Vector3 progressDirection = Vector3.forward;
                Selection.ThisShip.SetPosition(Vector3.MoveTowards(Selection.ThisShip.GetPosition(), Selection.ThisShip.GetPosition() + Selection.ThisShip.TransformDirection(progressDirection), progressDelta));

                //Selection.ThisShip.RotateModelDuringTurn(CurrentMovementData, PreviousMovementData);
                UpdateRotationFinisher();
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

        public void UpdateRotation()
        {

            float turningDirection = 0;
            if (Direction == ManeuverDirection.Right) turningDirection = 1;
            if (Direction == ManeuverDirection.Left) turningDirection = -1;

            Vector3 point_ShipStandBack = Selection.ThisShip.GetCentralBackPoint();
            Vector3 point_ShipStandFront = Selection.ThisShip.GetCentralFrontPoint();
            float pathToProcessLeft = (MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandBack).x);

            if (pathToProcessLeft > 0)
            {

                float distance_ShipStandFront_RulerStart = Vector3.Distance(MovementTemplates.CurrentTemplate.transform.position, point_ShipStandFront);
                float length_ShipStandFront_ShipStandBack = GetMovement1();
                Vector3 vector_RulerStart_ShipStandFront = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandFront);
                Vector3 vector_RulerStart_RulerBack = Vector3.right; // Strange magic due to ruler's rotation

                float angle_ToShipFront_ToRulerBack = Vector3.Angle(vector_RulerStart_ShipStandFront, vector_RulerStart_RulerBack);

                float sinSecondAngle = (distance_ShipStandFront_RulerStart / length_ShipStandFront_ShipStandBack) * Mathf.Sin(angle_ToShipFront_ToRulerBack * Mathf.Deg2Rad);
                float secondAngle = Mathf.Asin(sinSecondAngle) * Mathf.Rad2Deg;

                float angle_ToShipFront_CorrectStandPosition = -(180 - secondAngle - angle_ToShipFront_ToRulerBack);
                float rotationFix = angle_ToShipFront_CorrectStandPosition * turningDirection;
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, rotationFix, 0));

                Vector3 standOrientationVector = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.GetCentralFrontPoint()) - MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.GetCentralBackPoint());
                float angleBetweenMinus = -Vector3.Angle(vector_RulerStart_ShipStandFront, standOrientationVector);
                float angleFix = angleBetweenMinus * turningDirection;
                Selection.ThisShip.UpdateRotationHelperAngles(new Vector3(0, angleFix, 0));
            }

            MovementTemplates.AddRulerCenterPoint(point_ShipStandFront);

        }

        public void UpdateRotationFinisher()
        {
            if (MovementTemplates.CurrentTemplate.transform.Find("Finisher") != null) {

                Vector3 point_ShipStandBack = Selection.ThisShip.GetCentralBackPoint();
                Vector3 point_ShipStandFront = Selection.ThisShip.GetCentralFrontPoint();

                float pathToProcessFinishingLeft = (MovementTemplates.CurrentTemplate.transform.Find("Finisher").InverseTransformPoint(point_ShipStandFront).x);

                if (pathToProcessFinishingLeft > 0)
                {
                    float turningDirection = 0;
                    if (Direction == ManeuverDirection.Right) turningDirection = 1;
                    if (Direction == ManeuverDirection.Left) turningDirection = -1;

                    Vector3 point_NearestMovementRulerCenter = MovementTemplates.FindNearestRulerCenterPoint(point_ShipStandBack);

                    Vector3 vector_ShipBackStand_ShipStandFront = point_ShipStandFront - point_ShipStandBack;
                    Vector3 vector_NearestMovementRulerCenter_ShipStandFront = point_ShipStandFront - point_NearestMovementRulerCenter;
                    float angle_ToShipStandBack_ToNearestMovementRulerCenter = Vector3.Angle(vector_ShipBackStand_ShipStandFront, vector_NearestMovementRulerCenter_ShipStandFront);

                    Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, angle_ToShipStandBack_ToNearestMovementRulerCenter * turningDirection, 0));
                }

            }
        }

        protected override void PlanMovement()
        {
            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            float distancePart = ProgressTarget / 10;
            Vector3 position = Selection.ThisShip.GetPosition();

            GameObject lastShipStand = null;
            for (int i = 1; i <= 10; i++)
            {
                float step = (float)i * distancePart;
                GameObject ShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, position, Selection.ThisShip.GetRotation(), Board.GetBoard());

                float turningDirection = (Direction == ManeuverDirection.Right) ? 1 : -1;
                int progressDirection = 1;
                ShipStand.transform.RotateAround(Selection.ThisShip.TransformPoint(new Vector3(TurningAroundDistance * turningDirection, 0, 0)), new Vector3(0, 1, 0), turningDirection * step * progressDirection);

                if (i == 10) lastShipStand = ShipStand;
            }

            position = lastShipStand.transform.position;
            distancePart = GetMovement1() / 10;
            for (int i = 1; i <= 10; i++)
            {
                position = Vector3.MoveTowards(position, position + lastShipStand.transform.TransformDirection(Vector3.forward), distancePart);
                GameObject ShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, position, lastShipStand.transform.rotation, Board.GetBoard());
            }
        }

    }

}

