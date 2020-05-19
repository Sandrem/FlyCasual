using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class StraightMovement : GenericMovement
    {
        public StraightMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, MovementComplexity color) : base(speed, direction, bearing, color)
        {

        }

        public override IEnumerator Perform()
        {
            Initialize();

            movementPrediction = new MovementPrediction(TheShip, this);
            yield return movementPrediction.CalculateMovementPredicition();
        }

        protected override float SetProgressTarget()
        {
            return TheShip.ShipBase.GetShipBaseDistance() + Speed * GetMovement1();
        }

        protected override float SetAnimationSpeed()
        {
            return 10f;
        }

        public override void UpdateMovementExecution()
        {
            float progressDelta = AnimationSpeed * Time.deltaTime;
            progressDelta = Mathf.Clamp(progressDelta, 0, Mathf.Abs(ProgressTarget - ProgressCurrent));
            ProgressCurrent += progressDelta;

            TheShip.SetPosition(Vector3.MoveTowards(TheShip.GetPosition(), TheShip.GetPosition() + TheShip.TransformDirection(Vector3.forward), progressDelta));

            base.UpdateMovementExecution();
        }

        public override GameObject[] PlanMovement()
        {
            int precision = (IsSimple) ? 10 : 100;

            GameObject[] result = new GameObject[precision + 1];

            float distancePart = (TheShip.ShipBase.GetShipBaseDistance() + Speed * GetMovement1()) / (float)precision;
            Vector3 position = TheShip.GetPosition();

            for (int i = 0; i <= precision; i++)
            {
                if (i > 0) position = Vector3.MoveTowards(position, position + TheShip.TransformDirection(Vector3.forward), distancePart);
                GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
                GameObject ShipStand = MonoBehaviour.Instantiate(prefab, position, TheShip.GetRotation(), BoardTools.Board.GetBoard());

                Renderer[] renderers = ShipStand.GetComponentsInChildren<Renderer>();
                if (!DebugManager.DebugMovementShowTempBases)
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

        public override GameObject[] PlanFinalPosition()
        {
            GameObject[] result = new GameObject[1];

            float distance = (TheShip.ShipBase.GetShipBaseDistance() + Speed * GetMovement1());
            Vector3 position = TheShip.GetPosition();
            position += TheShip.TransformDirection(new Vector3(0,0, distance));

            GameObject prefab = (GameObject)Resources.Load(TheShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            GameObject ShipStand = MonoBehaviour.Instantiate(prefab, position, TheShip.GetRotation(), BoardTools.Board.GetBoard());
            ShipStand.name = "FinalPosition" + Speed;

            Renderer[] renderers = ShipStand.GetComponentsInChildren<Renderer>();
            if (!DebugManager.DebugMovementShowTempBases)
            {
                foreach (var render in renderers)
                {
                    render.enabled = false;
                }
            }

            result[0] = ShipStand;

            return result;
        }

        public override void AdaptSuccessProgress()
        {
            ProgressTarget *= movementPrediction.SuccessfullMovementProgress;
        }


    }

}

