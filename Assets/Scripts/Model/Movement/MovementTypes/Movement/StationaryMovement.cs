using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Movement
{

    public class StationaryMovement : GenericMovement
    {
        public StationaryMovement(int speed, ManeuverDirection direction, ManeuverBearing bearing, ManeuverColor color) : base(speed, direction, bearing, color)
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
            return 0;
        }

        protected override float SetAnimationSpeed()
        {
            return 1;
        }

        public override GameObject[] PlanMovement()
        {
            GameObject[] result = new GameObject[1];

            Vector3 position = TargetShip.GetPosition();

            GameObject prefab = (GameObject)Resources.Load(TargetShip.ShipBase.TemporaryPrefabPath, typeof(GameObject));
            GameObject ShipStand = MonoBehaviour.Instantiate(prefab, position, TargetShip.GetRotation(), Board.BoardManager.GetBoard());

            Renderer[] renderers = ShipStand.GetComponentsInChildren<Renderer>();
            foreach (var render in renderers)
            {
                render.enabled = false;
            }

            result[0] = ShipStand;

            return result;
        }

        public override void AdaptSuccessProgress() { }

    }

}

