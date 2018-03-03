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

            movementPrediction = new MovementPrediction(this, TargetShip.Owner.AfterShipMovementPrediction);
        }

        protected override float SetProgressTarget()
        {
            return TargetShip.ShipBase.GetShipBaseDistance() + Speed * GetMovement1();
        }

        protected override float SetAnimationSpeed()
        {
            return 5f;
        }

        public override void UpdateMovementExecution()
        {
            float progressDelta = AnimationSpeed * Time.deltaTime;
            progressDelta = Mathf.Clamp(progressDelta, 0, Mathf.Abs(ProgressTarget - ProgressCurrent));
            ProgressCurrent += progressDelta;

            TargetShip.SetPosition(Vector3.MoveTowards(TargetShip.GetPosition(), TargetShip.GetPosition() + TargetShip.TransformDirection(Vector3.forward), progressDelta));

            base.UpdateMovementExecution();
        }

        public override GameObject[] PlanMovement()
        {
            GameObject[] result = new GameObject[101];

            float distancePart = (TargetShip.ShipBase.GetShipBaseDistance() + Speed * GetMovement1())/100f;
            Vector3 position = TargetShip.GetPosition();

            for (int i = 0; i <= 100; i++)
            {
                if (i > 0) position = Vector3.MoveTowards(position, position + TargetShip.TransformDirection(Vector3.forward), distancePart);
                GameObject prefab = (GameObject)Resources.Load(TargetShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
                GameObject ShipStand = MonoBehaviour.Instantiate(prefab, position, TargetShip.GetRotation(), Board.BoardManager.GetBoard());

                Renderer[] renderers = ShipStand.GetComponentsInChildren<Renderer>();
                if (!DebugManager.DebugMovement)
                {
                    foreach (var render in renderers)
                    {
                        render.enabled = false;
                    }
                } 

                result[i] = ShipStand;
            }

            return result;
        }

        public override void AdaptSuccessProgress()
        {
            ProgressTarget *= movementPrediction.SuccessfullMovementProgress;
        }


    }

}

