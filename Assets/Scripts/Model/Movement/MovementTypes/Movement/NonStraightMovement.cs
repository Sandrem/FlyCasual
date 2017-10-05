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

        // GENERAL

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

        public override void AdaptSuccessProgress()
        {
            ProgressTarget *= (movementPrediction.SuccessfullMovementProgress >= 0.8f) ? 1f : 10f * movementPrediction.SuccessfullMovementProgress / 8f;
            finisherTargetSuccess *= (movementPrediction.SuccessfullMovementProgress >= 0.8f) ? 10f * (movementPrediction.SuccessfullMovementProgress - 0.8f) / 2f : 0f;
        }

        // MOVEMENT EXECUTION

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

                if (ProgressTarget != 0) Selection.ThisShip.RotateModelDuringTurn((ProgressCurrent / ProgressTarget) * (1 - 0.2f*finisherTargetSuccess));
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

            Selection.ThisShip.SimplifyRotationHelpers();
            movementFinisherLaunched = true;
        }

        // ROTATION

        public void UpdateRotation()
        {
            if (GetPathToProcessLeft(Selection.ThisShip.GetModelOrientation()) > 0)
            {
                float rotationFix = GetRotationFix(Selection.ThisShip.GetModelOrientation());
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, rotationFix, 0));

                float angleFix = GetAngleFix(Selection.ThisShip.GetModelOrientation());
                Selection.ThisShip.UpdateRotationHelperAngles(new Vector3(0, angleFix, 0));
            }

            AddMovementTemplateCenterPoint(Selection.ThisShip.Model);
        }

        public void UpdateRotationFinisher()
        {
            if (MovementTemplates.CurrentTemplate.transform.Find("Finisher") != null)
            {
                Selection.ThisShip.SimplifyRotationHelpers();
                bool isSuccessfull = TryRotateUsingStarter(Selection.ThisShip.GetModelOrientation());
                //bool isSuccessfull = false;

                if (!isSuccessfull)
                {
                    if (GetPathToProcessFinisherLeft(Selection.ThisShip.Model) > 0)
                    {
                        float angleToNearestCenterPoint = GetAngleToLastSavedTemplateCenterPoint(Selection.ThisShip.GetModelOrientation());
                        Selection.ThisShip.UpdateRotationHelper2Angles(new Vector3(0, angleToNearestCenterPoint * GetDirectionModifier(), 0));
                    }
                }
            }
        }

        //TEMPORARY
        public void UpdatePlanningRotationFinisher1(GameObject temporaryShipStand)
        {
            if (MovementTemplates.CurrentTemplate.transform.Find("Finisher") != null)
            {
                bool isSuccessfull = TryPlanningRotateUsingStarter(temporaryShipStand);

                if (!isSuccessfull)
                {
                    if (GetPathToProcessFinisherLeft(temporaryShipStand) > 0)
                    {
                        AdaptShipBaseToRotation(temporaryShipStand, lastPlanningRotation2);

                        float angleToNearestCenterPoint = GetAngleToLastSavedTemplateCenterPoint(temporaryShipStand);
                        temporaryShipStand.transform.localEulerAngles += new Vector3(0, angleToNearestCenterPoint * GetDirectionModifier(), 0);
                    }
                }
            }
        }

        // PLANNING

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

        // PLANNING ROTATION

        public void UpdatePlanningRotation(GameObject temporaryShipStand)
        {
            AdaptShipBaseToRotation(temporaryShipStand, lastPlanningRotation);

            float pathToProcessLeft = GetPathToProcessLeft(temporaryShipStand);

            AdaptShipBaseToRotation(temporaryShipStand, lastPlanningRotation, true);

            if (pathToProcessLeft > 0)
            {
                float rotationFix = GetRotationFix(temporaryShipStand);
                temporaryShipStand.transform.localEulerAngles += new Vector3(0, rotationFix, 0);

                float angleFix = GetAngleFix(temporaryShipStand);
                temporaryShipStand.transform.localEulerAngles += new Vector3(0, angleFix, 0);

                lastPlanningRotation = rotationFix + angleFix;
            }
            else
            {
                AdaptShipBaseToRotation(temporaryShipStand, lastPlanningRotation);
            }

            AddMovementTemplateCenterPoint(temporaryShipStand);
        }

        public void UpdatePlanningRotationFinisher(GameObject temporaryShipStand)
        {
            if (MovementTemplates.CurrentTemplate.transform.Find("Finisher") != null)
            {
                bool isSuccessfull = TryPlanningRotateUsingStarter(temporaryShipStand);

                if (!isSuccessfull)
                {
                    if (GetPathToProcessFinisherLeft(temporaryShipStand) > 0)
                    {
                        AdaptShipBaseToRotation(temporaryShipStand, lastPlanningRotation2);

                        float angleToNearestCenterPoint = GetAngleToLastSavedTemplateCenterPoint(temporaryShipStand);
                        temporaryShipStand.transform.localEulerAngles += new Vector3(0, angleToNearestCenterPoint * GetDirectionModifier(), 0);
                    }
                }
            }
        }

        private float GetAngleToLastSavedTemplateCenterPoint(GameObject shipBase)
        {
            float result = 0;

            Vector3 point_ShipStandBack = GetShipBaseBackPoint(shipBase);
            Vector3 point_ShipStandFront = shipBase.transform.TransformPoint(Vector3.zero);

            Vector3 point_NearestMovementRulerCenter = FindNearestRulerCenterPointUsingRotation(shipBase);

            Vector3 vector_ShipBackStand_ShipStandFront = point_ShipStandFront - point_ShipStandBack;
            Vector3 vector_NearestMovementRulerCenter_ShipStandFront = point_ShipStandFront - point_NearestMovementRulerCenter;
            result = GetDirectionModifier() * Vector3.SignedAngle(vector_ShipBackStand_ShipStandFront, vector_NearestMovementRulerCenter_ShipStandFront, Vector3.up);

            return result;
        }

        // SUBS

        private Vector3 FindNearestRulerCenterPointUsingRotation(GameObject shipBase)
        {
            Vector3 result = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 savedRotation = shipBase.transform.localEulerAngles;
            float minDistance = float.MaxValue;
            float distanceToNearestPoint = float.MaxValue;

            do
            {
                minDistance = distanceToNearestPoint;

                Vector3 point_ShipStandBack = GetShipBaseBackPoint(shipBase);
                Vector3 point_NearestMovementRulerCenter = MovementTemplates.FindNearestRulerCenterPoint(point_ShipStandBack);

                distanceToNearestPoint = Vector3.Distance(point_ShipStandBack, point_NearestMovementRulerCenter);

                if (distanceToNearestPoint < minDistance)
                {
                    result = point_NearestMovementRulerCenter;
                }

                shipBase.transform.localEulerAngles += new Vector3(0, 1 * GetDirectionModifier(), 0);

            }
            while (distanceToNearestPoint < minDistance);

            shipBase.transform.localEulerAngles = savedRotation;

            return result;
        }

        private Vector3 GetShipBaseBackPoint(GameObject shipBase)
        {
            return shipBase.transform.TransformPoint(new Vector3(0f, 0f, -2 * Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE));
        }

        private int GetDirectionModifier()
        {
            return (Direction == ManeuverDirection.Right) ? 1 : -1;
        }

        private void AdaptShipBaseToRotation(GameObject shipBase, float value, bool isReverse = false)
        {
            int modifier = (isReverse) ? -1 : 1;
            shipBase.transform.localEulerAngles += new Vector3(0, modifier * value, 0);
        }

        private float GetRotationFix(GameObject shipBase)
        {
            float result = 0;

            Vector3 point_ShipStandFront = shipBase.transform.TransformPoint(Vector3.zero);
            Vector3 vector_RulerStart_ShipStandFront = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandFront);

            float distance_ShipStandFront_RulerStart = Vector3.Distance(MovementTemplates.CurrentTemplate.transform.position, point_ShipStandFront);
            float length_ShipStandFront_ShipStandBack = Selection.ThisShip.ShipBase.GetShipBaseDistance();

            Vector3 vector_RulerStart_RulerBack = Vector3.right; // Strange magic due to ruler's rotation

            float angle_ToShipFront_ToRulerBack = Vector3.Angle(vector_RulerStart_ShipStandFront, vector_RulerStart_RulerBack);

            float sinSecondAngle = (distance_ShipStandFront_RulerStart / length_ShipStandFront_ShipStandBack) * Mathf.Sin(angle_ToShipFront_ToRulerBack * Mathf.Deg2Rad);
            float secondAngle = Mathf.Asin(sinSecondAngle) * Mathf.Rad2Deg;

            float angle_ToShipFront_CorrectStandPosition = -(180 - secondAngle - angle_ToShipFront_ToRulerBack);
            result = angle_ToShipFront_CorrectStandPosition * GetDirectionModifier();

            return result;
        }

        private float GetAngleFix(GameObject shipBase)
        {
            float result = 0;

            Vector3 point_ShipStandFront = shipBase.transform.TransformPoint(Vector3.zero);
            Vector3 vector_RulerStart_ShipStandFront = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandFront);

            Vector3 standOrientationVector = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.ShipBase.GetCentralFrontPoint()) - MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(Selection.ThisShip.ShipBase.GetCentralBackPoint());
            float angleBetweenMinus = -Vector3.Angle(vector_RulerStart_ShipStandFront, standOrientationVector);
            result = angleBetweenMinus * GetDirectionModifier();

            return result;
        }

        private static void AddMovementTemplateCenterPoint(GameObject shipBase)
        {
            Vector3 point_ShipStandFront = shipBase.transform.TransformPoint(Vector3.zero);
            MovementTemplates.AddRulerCenterPoint(point_ShipStandFront);
        }

        private float GetPathToProcessLeft(GameObject shipBase)
        {
            Vector3 point_ShipStandBack = GetShipBaseBackPoint(shipBase);
            return MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandBack).x;
        }

        private float GetPathToProcessFinisherLeft(GameObject shipBase)
        {
            Vector3 point_ShipStandFront = shipBase.transform.TransformPoint(Vector3.zero);
            return MovementTemplates.CurrentTemplate.transform.Find("Finisher").InverseTransformPoint(point_ShipStandFront).x;
        }

        private float TryGetRotationFixFinisher(GameObject shipStand)
        {
            float result = 0;

            Vector3 point_ShipStandFront = shipStand.transform.TransformPoint(Vector3.zero);

            float distance_ShipStandFront_RulerStart = Vector3.Distance(MovementTemplates.CurrentTemplate.transform.position, point_ShipStandFront);
            float length_ShipStandFront_ShipStandBack = Selection.ThisShip.ShipBase.GetShipBaseDistance();
            Vector3 vector_RulerStart_ShipStandFront = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandFront);
            Vector3 vector_RulerStart_RulerBack = Vector3.right; // Strange magic due to ruler's rotation

            float angle_ToShipFront_ToRulerBack = Vector3.Angle(vector_RulerStart_ShipStandFront, vector_RulerStart_RulerBack);

            float sinSecondAngle = (distance_ShipStandFront_RulerStart / length_ShipStandFront_ShipStandBack) * Mathf.Sin(angle_ToShipFront_ToRulerBack * Mathf.Deg2Rad);
            float secondAngle = Mathf.Asin(sinSecondAngle) * Mathf.Rad2Deg;

            float angle_ToShipFront_CorrectStandPosition = -(180 - secondAngle - angle_ToShipFront_ToRulerBack);
            result = angle_ToShipFront_CorrectStandPosition * GetDirectionModifier();

            return result;
        }

        private float GetAngleFixRotation(GameObject shipBase)
        {
            float result = 0;

            Vector3 point_ShipStandFront = shipBase.transform.TransformPoint(Vector3.zero);
            Vector3 vector_RulerStart_ShipStandFront = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(point_ShipStandFront);

            Vector3 standOrientationVector = MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(shipBase.transform.TransformPoint(Vector3.zero)) - MovementTemplates.CurrentTemplate.transform.InverseTransformPoint(shipBase.transform.TransformPoint(new Vector3(0f, 0f, -2 * Selection.ThisShip.ShipBase.HALF_OF_SHIPSTAND_SIZE)));
            float angleBetweenMinus = -Vector3.Angle(vector_RulerStart_ShipStandFront, standOrientationVector);
            result = angleBetweenMinus * -GetDirectionModifier();

            return result;
        }

        private bool TryPlanningRotateUsingStarter(GameObject shipBase)
        {
            AdaptShipBaseToRotation(shipBase, lastPlanningRotation);

            float rotationFix = TryGetRotationFixFinisher(shipBase);

            bool result = false;

            if (!float.IsNaN(rotationFix))
            {
                result = true;

                float angleFix = GetAngleFixRotation(shipBase);

                AdaptShipBaseToRotation(shipBase, rotationFix + angleFix);

                if (GetPathToProcessLeft(shipBase) > 0)
                {
                    lastPlanningRotation2 = rotationFix + angleFix;
                }
                else
                {
                    result = false;
                    AdaptShipBaseToRotation(shipBase, rotationFix + angleFix, true);
                }
            }

            return result;
        }

        private bool TryRotateUsingStarter(GameObject shipBase)
        {
            float rotationFix = TryGetRotationFixFinisher(shipBase);

            bool result = false;

            if (!float.IsNaN(rotationFix))
            {
                result = true;

                float angleFix = GetAngleFixRotation(shipBase);

                float savedRotation = Selection.ThisShip.GetModelOrientation().transform.localEulerAngles.y;
                Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, rotationFix, 0));
                Selection.ThisShip.UpdateRotationHelperAngles(new Vector3(0, angleFix, 0));

                if (GetPathToProcessLeft(shipBase) <= 0)
                {
                    result = false;

                    Selection.ThisShip.SetRotationHelper2Angles(new Vector3(0, savedRotation, 0));
                    Selection.ThisShip.UpdateRotationHelperAngles(new Vector3(0, -angleFix, 0));
                }
            }

            return result;
        }

    }

}

