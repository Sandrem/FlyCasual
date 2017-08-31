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

            movementPrediction = new MovementPrediction(this, Selection.ThisShip.Owner.AfterShipMovementPrediction);
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

            base.UpdateMovementExecution();
        }

        public override GameObject[] PlanMovement()
        {
            GameObject[] result = new GameObject[100];

            //TEMP
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            float distancePart = (GetMovement1() + Speed * GetMovement1())/100f;
            Vector3 position = Selection.ThisShip.GetPosition();

            for (int i = 1; i <= 100; i++)
            {
                position = Vector3.MoveTowards(position, position + Selection.ThisShip.TransformDirection(Vector3.forward), distancePart);
                GameObject ShipStand = MonoBehaviour.Instantiate(Game.Position.prefabShipStand, position, Selection.ThisShip.GetRotation(), Board.BoardManager.GetBoard());

                Renderer[] renderers = ShipStand.GetComponentsInChildren<Renderer>();
                foreach (var render in renderers)
                {
                    render.enabled = false;
                }

                result[i - 1] = ShipStand;
            }

            return result;
        }

        public override void AdaptSuccessProgress()
        {
            ProgressTarget *= movementPrediction.SuccessfullMovementProgress;
        }


    }

}

