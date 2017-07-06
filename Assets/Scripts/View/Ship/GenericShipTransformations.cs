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
            position = position + Model.transform.TransformVector(0, 0, HALF_OF_SHIPSTAND_SIZE);
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
            result = Model.transform.TransformPoint(0, 0, -HALF_OF_SHIPSTAND_SIZE);
            return result;
        }

        public Dictionary<string, float> GetBounds()
        {
            List<Vector3> edgesList = new List<Vector3>();
            edgesList.Add(Model.transform.TransformPoint(standEdgePoins["RF"]));
            edgesList.Add(Model.transform.TransformPoint(standEdgePoins["LF"]));
            edgesList.Add(Model.transform.TransformPoint(standEdgePoins["RB"]));
            edgesList.Add(Model.transform.TransformPoint(standEdgePoins["LB"]));

            Dictionary<string, float> bounds = new Dictionary<string, float>();
            bounds.Add("minX", Mathf.Min(edgesList[0].x, edgesList[1].x, edgesList[2].x, edgesList[3].x));
            bounds.Add("maxX", Mathf.Max(edgesList[0].x, edgesList[1].x, edgesList[2].x, edgesList[3].x));
            bounds.Add("minZ", Mathf.Min(edgesList[0].z, edgesList[1].z, edgesList[2].z, edgesList[3].z));
            Debug.Log(edgesList[0].z + " + " + edgesList[1].z + " + " + edgesList[2].z + " + " + edgesList[3].z);
            bounds.Add("maxZ", Mathf.Max(edgesList[0].z, edgesList[1].z, edgesList[2].z, edgesList[3].z));

            return bounds;
        }

        public Vector3 GetModelCenter()
        {
            return modelCenter.position;
        }

        // ROTATION HELPERS

        public void UpdateRotationHelperAngles(Vector3 angles)
        {
            Model.transform.Find("RotationHelper").localEulerAngles = new Vector3(angles.x, angles.y + Model.transform.Find("RotationHelper").localEulerAngles.y, angles.z);
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

        public void Rotate(Vector3 point, float progress)
        {
            Model.transform.RotateAround(point, new Vector3(0, 1, 0), progress);
        }

        public void Rotate180()
        {
            Model.transform.RotateAround(Model.transform.TransformPoint(new Vector3(0, 0, -HALF_OF_SHIPSTAND_SIZE)), Vector3.up, 180);
        }

        public void RotateModelDuringTurn(MovementExecutionData currentMovementData, MovementExecutionData previousMovementData)
        {
            if ((currentMovementData.MovementDirection == ManeuverDirection.Forward) && (previousMovementData.Speed == 0)) return;
            if (currentMovementData.CollisionReverting) return;

            float progressCurrent = currentMovementData.CurrentProgress;
            float progressTarget = currentMovementData.TargetProgress;
            float turningDirection = 0;

            if (currentMovementData.MovementDirection != ManeuverDirection.Forward)
            {
                progressTarget += progressTarget * (1f / currentMovementData.Speed);
                turningDirection = (currentMovementData.MovementDirection == ManeuverDirection.Right) ? 1 : -1;
            }
            if (previousMovementData.Speed != 0)
            {
                progressCurrent += progressTarget * previousMovementData.Speed;
                progressTarget += progressTarget * previousMovementData.Speed;
                turningDirection = (previousMovementData.MovementDirection == ManeuverDirection.Right) ? 1 : -1;
            }

            float progress = progressCurrent / progressTarget;
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

    }

}
