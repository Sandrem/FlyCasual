﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class ReverseStraightMovement : GenericMovement
    {
        public ReverseStraightMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, MovementComplexity color) : base(speed, direction, bearing, color)
        {

        }

        public override void Perform()
        {
            base.Perform();
            Initialize();

            Rotate180Hidden();

            movementPrediction = new MovementPrediction(this, TheShip.Owner.AfterShipMovementPrediction);
        }

        private void Rotate180Hidden()
        {
            Selection.ThisShip.Model.transform.eulerAngles += new Vector3(0, 180, 0);
            Selection.ThisShip.Model.transform.position = Vector3.MoveTowards(
                Selection.ThisShip.Model.transform.position,
                Selection.ThisShip.Model.transform.position + Selection.ThisShip.Model.transform.forward,
                BoardTools.Board.BoardIntoWorld(Selection.ThisShip.ShipBase.SHIPSTAND_SIZE_CM)
            );
            Selection.ThisShip.GetShipAllPartsTransform().eulerAngles += new Vector3(0, 180, 0);
        }

        protected override float SetProgressTarget()
        {
            return TheShip.ShipBase.GetShipBaseDistance() + Speed * GetMovement1();
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

            TheShip.SetPosition(Vector3.MoveTowards(TheShip.GetPosition(), TheShip.GetPosition() + TheShip.TransformDirection(Vector3.forward), progressDelta));

            base.UpdateMovementExecution();
        }

        public override GameObject[] PlanMovement()
        {
            GameObject[] result = new GameObject[101];

            float distancePart = (TheShip.ShipBase.GetShipBaseDistance() + Speed * GetMovement1())/100f;
            Vector3 position = TheShip.GetPosition();

            for (int i = 0; i <= 100; i++)
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

            position = Vector3.MoveTowards(position, position + TheShip.TransformDirection(Vector3.forward), distance);

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

            result[0] = ShipStand;

            return result;
        }

        public override void AdaptSuccessProgress()
        {
            ProgressTarget *= movementPrediction.SuccessfullMovementProgress;
        }

        protected override void ManeuverEndRotation(Action callBack)
        {
            Rotate180Hidden();
            callBack();
        }
    }

}

