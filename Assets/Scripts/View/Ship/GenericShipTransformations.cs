using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {
        // POSITION AND ANGLES

        public void SetPosition(Vector3 position)
        {
            Model.transform.position = position;
        }

        public void SetCenter(Vector3 position)
        {
            position = position + Model.transform.TransformVector(0, 0, ShipBase.HALF_OF_SHIPSTAND_SIZE);
            Model.transform.position = position;
        }

        public Vector3 GetPosition()
        {
            return Model.transform.position;
        }

        public Vector3 GetAngles()
        {
            return Model.transform.eulerAngles;
        }

        public void SetAngles(Vector3 angles)
        {
            Model.transform.eulerAngles = angles;
        }

        public Quaternion GetRotation()
        {
            return Model.transform.rotation;
        }

        public Vector3 GetCenter()
        {
            Vector3 result;
            result = Model.transform.TransformPoint(0, 0, -ShipBase.HALF_OF_SHIPSTAND_SIZE);
            return result;
        }

        public Vector3 GetBack()
        {
            Vector3 result;
            result = Model.transform.TransformPoint(0, 0, -2*ShipBase.HALF_OF_SHIPSTAND_SIZE);
            return result;
        }

        public Vector3 GetModelCenter()
        {
            return modelCenter.position;
        }

        public GameObject GetModelOrientation()
        {
            return Model.transform.Find("RotationHelper/RotationHelper2").gameObject;
        }           

        // ROTATION HELPERS

        public void UpdateRotationHelperAngles(Vector3 angles)
        {
            Model.transform.Find("RotationHelper").localEulerAngles = new Vector3(angles.x, angles.y + Model.transform.Find("RotationHelper").localEulerAngles.y, angles.z);
        }

        public void UpdateRotationHelper2Angles(Vector3 angles)
        {
            Model.transform.Find("RotationHelper/RotationHelper2").localEulerAngles = new Vector3(angles.x, angles.y + Model.transform.Find("RotationHelper/RotationHelper2").localEulerAngles.y, angles.z);
        }

        public void SetRotationHelper2Angles(Vector3 angles)
        {
            Model.transform.Find("RotationHelper/RotationHelper2").localEulerAngles = angles;
        }

        public void ApplyRotationHelpers()
        {
            Model.transform.localEulerAngles += Model.transform.Find("RotationHelper").localEulerAngles + Model.transform.Find("RotationHelper/RotationHelper2").localEulerAngles;
        }

        public void ResetRotationHelpers()
        {
            Model.transform.Find("RotationHelper").localEulerAngles = Vector3.zero;
            Model.transform.Find("RotationHelper/RotationHelper2").localEulerAngles = Vector3.zero;
        }

        public void SimplifyRotationHelpers()
        {
            Model.transform.Find("RotationHelper").localEulerAngles += Model.transform.Find("RotationHelper/RotationHelper2").localEulerAngles;
            Model.transform.Find("RotationHelper/RotationHelper2").localEulerAngles = Vector3.zero;
        }

        // TRANSFORMATIONS

        public Vector3 TransformDirection(Vector3 direction)
        {
            return Model.transform.TransformDirection(direction);
        }

        public Vector3 TransformVector(Vector3 vector)
        {
            return Model.transform.TransformDirection(vector);
        }

        public Vector3 TransformPoint(Vector3 point)
        {
            return Model.transform.TransformPoint(point);
        }

        public Vector3 InverseTransformPoint(Vector3 point)
        {
            return Model.transform.InverseTransformPoint(point);
        }

        // ROTATION

        public void RotateAround(Vector3 point, float progress)
        {
            Model.transform.RotateAround(point, new Vector3(0, 1, 0), progress);
        }

        public void Rotate180()
        {
            Model.transform.RotateAround(Model.transform.TransformPoint(new Vector3(0, 0, -ShipBase.HALF_OF_SHIPSTAND_SIZE)), Vector3.up, 180);
        }

        public void RotateModelDuringTurn(float progress, Movement.ManeuverDirection direction)
        {
            float turningDirection = 0;
            turningDirection = (direction == Movement.ManeuverDirection.Right) ? 1 : -1;

            if (progress > 0.5f)
            {
                progress = 1 - progress;
            }
            modelCenter.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 45 * turningDirection, progress));
        }

        public void RotateModelDuringBarrelRoll(float progress, float turningDirection)
        {
            modelCenter.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, turningDirection * 360, progress));
        }

        // S-Foils

        public void WingsOpen()
        {
            if ((this as IMovableWings).CurrentWingsPosition == WingsPositions.Closed)
            {
                WingsChangePosition("Open");
                (this as IMovableWings).CurrentWingsPosition = WingsPositions.Opened;
            }
        }

        public void WingsClose()
        {
            if ((this as IMovableWings).CurrentWingsPosition == WingsPositions.Opened)
            {
                WingsChangePosition("Close");
                (this as IMovableWings).CurrentWingsPosition = WingsPositions.Closed;
            }
        }

        private void WingsChangePosition(string wingPosition)
        {
            if (!(this is IMovableWings))
            {
                Console.Write("Ship doesn't have movable wings!", LogTypes.Errors, true, "red");
                return;
            }

            Sounds.PlayShipSound("Servomotors", this);

            foreach (Transform transform in GetModelTransform())
            {
                if (!transform.name.StartsWith("Wing")) continue;

                Animation wingAnimator = transform.GetComponent<Animation>();
                wingAnimator.Play(transform.name + "_" + wingPosition);
            }
        }

    }

}
