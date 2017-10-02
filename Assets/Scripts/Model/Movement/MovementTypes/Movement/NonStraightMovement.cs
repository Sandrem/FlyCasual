using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public abstract class NonStraightMovement : GenericMovement
    {
        protected float turningAroundDistance;
        protected float finisherTargetSuccess = 1;
        protected bool movementFinisherLaunched;
        private float lastPlanningRotation = 0;
        private float lastPlanningRotation2 = 0;

        public NonStraightMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            base.Perform();
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            turningAroundDistance = SetTurningAroundDistance();
        }

        protected virtual float SetTurningAroundDistance()
        {
            return 0;
        }

        public override void UpdateMovementExecution()
        {
            float progressDelta = AnimationSpeed * Time.deltaTime;
            if (DebugManager.DebugMovement) progressDelta *= 0.1f;

            progressDelta = Mathf.Clamp(progressDelta, 0, Mathf.Abs(ProgressTarget - ProgressCurrent));
            ProgressCurrent += progressDelta;

            if (!movementFinisherLaunched)
            {
                float turningDirection = (Direction == ManeuverDirection.Right) ? 1 : -1;

                int progressDirection = 1;
                Selection.ThisShip.RotateAround(Selection.ThisShip.TransformPoint(new Vector3(turningAroundDistance * turningDirection, 0, 0)), turningDirection * progressDelta * progressDirection);

                Selection.ThisShip.RotateModelDuringTurn((ProgressCurrent / ProgressTarget) * (1 - 0.2f*finisherTargetSuccess));
                UpdateRotation();
            }
            else
            {
                Vector3 progressDirection = Vector3.forward;
                Selection.ThisShip.SetPosition(Vector3.MoveTowards(Selection.ThisShip.GetPosition(), Selection.ThisShip.GetPosition() + Selection.ThisShip.TransformDirection(progressDirection), progressDelta));

                if (finisherTargetSuccess != 0) Selection.ThisShip.RotateModelDuringTurn((1 - 0.2f * finisherTargetSuccess) + (ProgressCurrent / ProgressTarget) * 0.2f);
                UpdateRotationFinisher();
            }

            base.UpdateMovementExecution();
        }

        protected override void CheckFinish()
        {
            if (ProgressTarget == ProgressCurrent)
            {
                if (movementFinisherLaunched)
                {
                    FinishMovement();
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

            Vector3 TargetPosition = new Vector3(0, 0, Selection.ThisShip.ShipBase.GetShipBaseDistance());
            ProgressTarget = TargetPosition.z * finisherTargetSuccess;

            AnimationSpeed = Options.ManeuverSpeed * 0.75f;

            movementFinisherLaunched = true;
        }

        public void UpdateRotation()
        {

            float turningDirection = 0;
            if (Direction == ManeuverDirection.Right) turningDirection = 1;
            if (Direction == ManeuverDirection.Left) turningDirection = -1;

            Vector3 point_ShipStandBack = Selection.ThisShip.ShipBase.GetCentralBackPoint();
            Vector3 point_ShipStandFront = Selection.ThisShip.ShipBase.GetCentralFrontPoint();
            float pathToProcessLeft = (MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandBack).x);

            if (pathToProcessLeft > 0)
            {

                float distance_ShipStandFront_RulerStart = Vector3.Distance(MovementTemplates.CurrentTemplate.transform.position, point_ShipStandFront);
                float length_ShipStandFront_ShipStandBack = Selection.ThisShip.ShipBase.GetShipBaseDistance();
                Vector3 vector_RulerStart_ShipStandFront = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandFront);
                Vector3 vector_RulerStart_RulerBack = Vector3.right; // Strange magic due to ruler's rotation

                float angle_ToShipFront_ToRulerBack = Vector3.Angle(vector_RulerStart_ShipStandFront, vector_RulerStart_RulerBack);

                float sinSecondAngle = (distance_ShipStandFront_RulerStart / length_ShipStandFront_ShipStandBack) * Mathf.Sin(angle_ToShipFront_ToRulerBack * Mathf.Deg2Rad);
                float secondAngle = Mathf.Asin(sinSecondAngle) * Mathf.Rad2Deg;

                float angle_ToShipFront_CorrectStandPosition = -(180 - secondAngle - angle_ToShipFront_ToRulerBack);
                float rotationFix = angle_ToShipFront_CorrectStandPosition * turningDirection;
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, rotationFix, 0));

                Vector3 standOrientationVector = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.ShipBase.GetCentralFrontPoint()) - MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.ShipBase.GetCentralBackPoint());
                float angleBetweenMinus = -Vector3.Angle(vector_RulerStart_ShipStandFront, standOrientationVector);
                float angleFix = angleBetweenMinus * turningDirection;
                Selection.ThisShip.UpdateRotationHelperAngles(new Vector3(0, angleFix, 0));
            }

            MovementTemplates.AddRulerCenterPoint(point_ShipStandFront);

        }

        public void UpdatePlanningRotation(GameObject temporaryShipStand)
        {

            float turningDirection = 0;
            if (Direction == ManeuverDirection.Right) turningDirection = 1;
            if (Direction == ManeuverDirection.Left) turningDirection = -1;

            temporaryShipStand.transform.localEulerAngles += new Vector3(0, lastPlanningRotation, 0);

            Vector3 point_ShipStandBack = temporaryShipStand.transform.TransformPoint(new Vector3(0f, 0f, -2 * Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE));
            Debug.Log("point_ShipStandBack: " + point_ShipStandBack.x + " " + point_ShipStandBack.z);
            //Selection.ThisShip.ShipBase.GetCentralBackPoint();
            Vector3 point_ShipStandFront = temporaryShipStand.transform.TransformPoint(Vector3.zero);
            //Selection.ThisShip.ShipBase.GetCentralFrontPoint();

            temporaryShipStand.transform.localEulerAngles -= new Vector3(0, lastPlanningRotation, 0);

            float pathToProcessLeft = (MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandBack).x);

            if (pathToProcessLeft > 0)
            {
                float distance_ShipStandFront_RulerStart = Vector3.Distance(MovementTemplates.CurrentTemplate.transform.position, point_ShipStandFront);
                float length_ShipStandFront_ShipStandBack = Selection.ThisShip.ShipBase.GetShipBaseDistance();
                Vector3 vector_RulerStart_ShipStandFront = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandFront);
                Vector3 vector_RulerStart_RulerBack = Vector3.right; // Strange magic due to ruler's rotation

                float angle_ToShipFront_ToRulerBack = Vector3.Angle(vector_RulerStart_ShipStandFront, vector_RulerStart_RulerBack);

                float sinSecondAngle = (distance_ShipStandFront_RulerStart / length_ShipStandFront_ShipStandBack) * Mathf.Sin(angle_ToShipFront_ToRulerBack * Mathf.Deg2Rad);
                float secondAngle = Mathf.Asin(sinSecondAngle) * Mathf.Rad2Deg;

                float angle_ToShipFront_CorrectStandPosition = -(180 - secondAngle - angle_ToShipFront_ToRulerBack);
                float rotationFix = angle_ToShipFront_CorrectStandPosition * turningDirection;
                temporaryShipStand.transform.localEulerAngles += new Vector3(0, rotationFix, 0);
                //Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, rotationFix, 0));


                Vector3 standOrientationVector = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.ShipBase.GetCentralFrontPoint()) - MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.ShipBase.GetCentralBackPoint());
                float angleBetweenMinus = -Vector3.Angle(vector_RulerStart_ShipStandFront, standOrientationVector);
                float angleFix = angleBetweenMinus * turningDirection;
                temporaryShipStand.transform.localEulerAngles += new Vector3(0, angleFix, 0);
                //Selection.ThisShip.UpdateRotationHelperAngles(new Vector3(0, angleFix, 0));

                lastPlanningRotation = rotationFix + angleFix;
            }
            else
            {
                temporaryShipStand.transform.localEulerAngles += new Vector3(0, lastPlanningRotation, 0);
                //Debug.Log(lastPlanningRotation);*/
            }

            MovementTemplates.AddRulerCenterPoint(point_ShipStandFront);

        }

        public void UpdateRotationFinisher()
        {
            if (MovementTemplates.CurrentTemplate.transform.Find("Finisher") != null) {

                Vector3 point_ShipStandBack = Selection.ThisShip.ShipBase.GetCentralBackPoint();
                Vector3 point_ShipStandFront = Selection.ThisShip.ShipBase.GetCentralFrontPoint();

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

                    //Debug.Log("Execution: current is " + Selection.ThisShip.Model.transform.localEulerAngles.y + ", adaptation is " + -angle_ToShipStandBack_ToNearestMovementRulerCenter);
                }

            }
        }

        public void UpdatePlanningRotationFinisher(GameObject temporaryShipStand)
        {
            if (MovementTemplates.CurrentTemplate.transform.Find("Finisher") != null)
            {
                //Debug.Log("lastPlanningRotation: " + lastPlanningRotation);
                temporaryShipStand.transform.localEulerAngles += new Vector3(0f, lastPlanningRotation+lastPlanningRotation2, 0f);

                Vector3 point_ShipStandBack = temporaryShipStand.transform.TransformPoint(new Vector3(0f, 0f, -2 * Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE));
                //Selection.ThisShip.ShipBase.GetCentralBackPoint();
                Vector3 point_ShipStandFront = temporaryShipStand.transform.TransformPoint(Vector3.zero);
                //Selection.ThisShip.ShipBase.GetCentralFrontPoint();

                float pathToProcessLeft = (MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandBack).x);
                Debug.Log(pathToProcessLeft);

                if (pathToProcessLeft < 0)
                {
                    float pathToProcessFinishingLeft = (MovementTemplates.CurrentTemplate.transform.Find("Finisher").InverseTransformPoint(point_ShipStandFront).x);

                    if (pathToProcessFinishingLeft > 0)
                    {
                        float turningDirection = 0;
                        if (Direction == ManeuverDirection.Right) turningDirection = 1;
                        if (Direction == ManeuverDirection.Left) turningDirection = -1;

                        Vector3 point_NearestMovementRulerCenter = MovementTemplates.FindNearestRulerCenterPoint(point_ShipStandBack);
                        //Debug.Log("Back: " + point_ShipStandBack.x + " " + point_ShipStandBack.z + " / Nearest: " + point_NearestMovementRulerCenter.x + " " + point_NearestMovementRulerCenter.z);

                        Vector3 vector_ShipBackStand_ShipStandFront = point_ShipStandFront - point_ShipStandBack;
                        Vector3 vector_NearestMovementRulerCenter_ShipStandFront = point_ShipStandFront - point_NearestMovementRulerCenter;
                        float angle_ToShipStandBack_ToNearestMovementRulerCenter = Vector3.Angle(vector_ShipBackStand_ShipStandFront, vector_NearestMovementRulerCenter_ShipStandFront);

                        temporaryShipStand.transform.localEulerAngles += new Vector3(0, angle_ToShipStandBack_ToNearestMovementRulerCenter * turningDirection, 0);
                        lastPlanningRotation2 += angle_ToShipStandBack_ToNearestMovementRulerCenter * turningDirection;
                        //lastPlanningRotation += angle_ToShipStandBack_ToNearestMovementRulerCenter;
                        //Debug.Log("Planning: current is " + temporaryShipStand.transform.localEulerAngles.y + ", adaptation is " + -angle_ToShipStandBack_ToNearestMovementRulerCenter);
                    }
                }
                else
                {
                    /*float distance_ShipStandFront_RulerStart = Vector3.Distance(MovementTemplates.CurrentTemplate.transform.position, point_ShipStandFront);
                    float length_ShipStandFront_ShipStandBack = Selection.ThisShip.ShipBase.GetShipBaseDistance();
                    Vector3 vector_RulerStart_ShipStandFront = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandFront);
                    Vector3 vector_RulerStart_RulerBack = Vector3.right; // Strange magic due to ruler's rotation

                    float angle_ToShipFront_ToRulerBack = Vector3.Angle(vector_RulerStart_ShipStandFront, vector_RulerStart_RulerBack);

                    float sinSecondAngle = (distance_ShipStandFront_RulerStart / length_ShipStandFront_ShipStandBack) * Mathf.Sin(angle_ToShipFront_ToRulerBack * Mathf.Deg2Rad);
                    float secondAngle = Mathf.Asin(sinSecondAngle) * Mathf.Rad2Deg;

                    float angle_ToShipFront_CorrectStandPosition = -(180 - secondAngle - angle_ToShipFront_ToRulerBack);
                    float rotationFix = angle_ToShipFront_CorrectStandPosition * turningDirection;
                    temporaryShipStand.transform.localEulerAngles += new Vector3(0, rotationFix, 0);
                    //Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, rotationFix, 0));


                    Vector3 standOrientationVector = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.ShipBase.GetCentralFrontPoint()) - MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.ShipBase.GetCentralBackPoint());
                    float angleBetweenMinus = -Vector3.Angle(vector_RulerStart_ShipStandFront, standOrientationVector);
                    float angleFix = angleBetweenMinus * turningDirection;
                    temporaryShipStand.transform.localEulerAngles += new Vector3(0, angleFix, 0);
                    //Selection.ThisShip.UpdateRotationHelperAngles(new Vector3(0, angleFix, 0));

                    lastPlanningRotation = rotationFix + angleFix;*/
                }
            }
        }

        public override GameObject[] PlanMovement()
        {
            //Temporary
            MovementTemplates.ApplyMovementRuler(Selection.ThisShip);

            GameObject[] result = new GameObject[100];

            float distancePart = ProgressTarget / 80;
            Vector3 position = Selection.ThisShip.GetPosition();

            GameObject lastShipStand = null;
            for (int i = 1; i <= 80; i++)
            {
                float step = (float)i * distancePart;
                GameObject prefab = (GameObject)Resources.Load(Selection.ThisShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
                GameObject ShipStand = MonoBehaviour.Instantiate(prefab, position, Selection.ThisShip.GetRotation(), Board.BoardManager.GetBoard());

                Renderer[] renderers = ShipStand.GetComponentsInChildren<Renderer>();
                if (!DebugManager.DebugMovement)
                {
                    foreach (var render in renderers)
                    {
                        render.enabled = false;
                    }
                }

                float turningDirection = (Direction == ManeuverDirection.Right) ? 1 : -1;
                int progressDirection = 1;
                ShipStand.transform.RotateAround(Selection.ThisShip.TransformPoint(new Vector3(turningAroundDistance * turningDirection, 0, 0)), new Vector3(0, 1, 0), turningDirection * step * progressDirection);

                UpdatePlanningRotation(ShipStand);

                if (i == 80) lastShipStand = ShipStand;

                result[i - 1] = ShipStand;

                ShipStand.name = "Main" + i;
            }

            GameObject savedShipStand = MonoBehaviour.Instantiate(lastShipStand);
            savedShipStand.transform.localEulerAngles -= new Vector3(0f, lastPlanningRotation, 0f);

            position = lastShipStand.transform.position;
            distancePart = Selection.ThisShip.ShipBase.GetShipBaseDistance() / 20;
            for (int i = 1; i <= 20; i++)
            {
                position = Vector3.MoveTowards(position, position + savedShipStand.transform.TransformDirection(Vector3.forward), distancePart);
                GameObject prefab = (GameObject)Resources.Load(Selection.ThisShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
                GameObject ShipStand = MonoBehaviour.Instantiate(prefab, position, savedShipStand.transform.rotation, Board.BoardManager.GetBoard());

                Renderer[] renderers = ShipStand.GetComponentsInChildren<Renderer>();
                if (!DebugManager.DebugMovement)
                {
                    foreach (var render in renderers)
                    {
                        render.enabled = false;
                    }
                }

                UpdatePlanningRotationFinisher(ShipStand);

                result[i + 80 - 1] = ShipStand;

                ShipStand.name = "Finishing" + i;
            }

            MonoBehaviour.Destroy(savedShipStand);

            MovementTemplates.HideLastMovementRuler();

            return result;
        }

        public override void AdaptSuccessProgress()
        {
            ProgressTarget *= (movementPrediction.SuccessfullMovementProgress >= 0.8f) ? 1f : 10f * movementPrediction.SuccessfullMovementProgress / 8f;
            finisherTargetSuccess *= (movementPrediction.SuccessfullMovementProgress >= 0.8f) ? 10f * (movementPrediction.SuccessfullMovementProgress - 0.8f) / 2f : 0f;
        }

    }

}

