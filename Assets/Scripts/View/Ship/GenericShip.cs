using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {

        private Dictionary<string, Vector3> standFrontEdgePoins = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standEdgePoins = new Dictionary<string, Vector3>();
        private const float HALF_OF_SHIPSTAND_SIZE = 0.5f;
        private const float SHIPSTAND_SIZE = 1f;

        public void CreateModel(Vector3 position)
        {
            Model = CreateShipModel(position);

            standFrontEdgePoins.Add("LF", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, 0));
            standFrontEdgePoins.Add("RF", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, 0));

            standEdgePoins.Add("LF", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, 0));
            standEdgePoins.Add("CF", Vector3.zero);
            standEdgePoins.Add("RF", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, 0));
            standEdgePoins.Add("LB", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoins.Add("CB", new Vector3(0, 0, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoins.Add("RB", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
        }

        public GameObject CreateShipModel(Vector3 position)
        {

            Vector3 facing = (Owner.PlayerNo == Players.PlayerNo.Player1) ? ShipFactory.ROTATION_FORWARD : ShipFactory.ROTATION_BACKWARD;

            position = new Vector3(0, 0, (Owner.PlayerNo == Players.PlayerNo.Player1) ? -4 : 4);

            GameObject newShip = MonoBehaviour.Instantiate(Game.PrefabsList.ShipModel, position + new Vector3(0, 0.03f, 0), Quaternion.Euler(facing), Board.GetBoard());
            newShip.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).gameObject.SetActive(true);

            ShipId = ShipFactory.lastId;
            ShipFactory.lastId = ShipFactory.lastId + 1;
            SetTagOfChildren(newShip.transform, "ShipId:" + ShipId.ToString());

            //Check Size of stand
            //Debug.Log(Board.transform.InverseTransformPoint(newShip.transform.TransformPoint(new Vector3(-0.5f, -0.5f, -0.5f))));
            //Debug.Log(Board.transform.InverseTransformPoint(newShip.transform.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f))));

            //Check size of playmat
            //Debug.Log(Board.transform.InverseTransformPoint(Board.transform.Find("Playmat").transform.TransformPoint(new Vector3(-0.5f, -0.5f, -0.5f))));
            //Debug.Log(Board.transform.InverseTransformPoint(Board.transform.Find("Playmat").transform.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f))));

            return newShip;
        }

        private static void SetTagOfChildren(Transform parent, string tag)
        {
            parent.gameObject.tag = tag;
            if (parent.childCount > 0)
            {
                foreach (Transform t in parent)
                {
                    SetTagOfChildren(t, tag);
                }
            }
        }

        public void SetShipInstertImage()
        {
            string materialName = PilotName;
            materialName = materialName.Replace(' ', '_');
            materialName = materialName.Replace('"', '_');
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = (Material)Resources.Load("ShipStandInsert/Materials/" + materialName, typeof(Material));
        }

        public Vector3 GetAngles()
        {
            return Model.transform.eulerAngles;
        }

        public void SetAngles(Vector3 angles)
        {
            Model.transform.eulerAngles = angles;
        }

        public void UpdateRotationHelperAngles(Vector3 angles)
        {
            Model.transform.Find("RotationHelper").localEulerAngles = new Vector3(angles.x, angles.y + Model.transform.Find("RotationHelper").localEulerAngles.y, angles.z);
        }

        public void SetRotationHelper2Angles(Vector3 angles)
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles = angles;
        }

        public void SetPosition(Vector3 position)
        {
            Model.transform.position = position;
        }

        public Vector3 GetPosition()
        {
            return Model.transform.position;
        }

        public Quaternion GetRotation()
        {
            return Model.transform.rotation;
        }

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

        public void Rotate(Vector3 point, float progress)
        {
            Model.transform.RotateAround(point, new Vector3(0, 1, 0), progress);
        }

        public ShipStandCollider GetShipStandScript()
        {
            return Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").GetComponent<ShipStandCollider>();
        }

        public void ApplyShader(string type)
        {
            if (type == "default")
            {
                foreach (Transform transform in Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type))
                {
                    //Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("Spotlight").gameObject.SetActive(false);
                    //transform.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                }
            }
            if (type == "selectedYellow")
            {
                foreach (Transform transform in Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type))
                {
                    //Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("Spotlight").gameObject.SetActive(true);
                    //transform.GetComponent<Renderer>().material.shader = Shader.Find("Outlined/Silhouetted Diffuse");
                    //transform.GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.yellow);
                }
            }
            if (type == "selectedRed")
            {
                foreach (Transform transform in Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type))
                {
                    //Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("Spotlight").gameObject.SetActive(true);
                    //transform.GetComponent<Renderer>().material.shader = Shader.Find("Outlined/Silhouetted Diffuse");
                    //transform.GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.red);
                }
            }
        }

        public void SetActive(bool argument)
        {
            Model.SetActive(argument);
        }

        public string GetTag()
        {
            return Model.tag;
        }

        public Vector3 GetFrontFacing()
        {
            return Model.transform.TransformDirection(0, 0, 1f);
        }

        public Vector3 GetCentralFrontPoint()
        {
            return Model.transform.Find("RotationHelper").TransformPoint(standEdgePoins["CF"]);
        }

        public Vector3 GetCentralBackPoint()
        {
            return Model.transform.Find("RotationHelper").TransformPoint(standEdgePoins["CB"]);
        }

        public void ApplyRotationHelpers()
        {
            Model.transform.localEulerAngles += Model.transform.Find("RotationHelper").localEulerAngles + Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles;
        }

        public void ResetRotationHelpers()
        {
            Model.transform.Find("RotationHelper").localEulerAngles = Vector3.zero;
            Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles = Vector3.zero;
        }

        public void SimplifyRotationHelpers()
        {
            Model.transform.Find("RotationHelper").localEulerAngles += Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles;
            Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles = Vector3.zero;
        }

        public void Rotate180()
        {
            Model.transform.RotateAround(Model.transform.TransformPoint(new Vector3(0, 0, -HALF_OF_SHIPSTAND_SIZE)), Vector3.up, 180);
        }

        public void SetRaycastTarget(bool value)
        {
            int layer = (value) ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("Ignore Raycast") ;
            SetLayerRecursive(Model.transform, layer);
        }

        private void SetLayerRecursive(Transform target, int layer)
        {
            target.gameObject.layer = layer;
            foreach (Transform transform in target)
            {
                SetLayerRecursive(transform, layer);
            }
        }

        public Dictionary<string, Vector3> GetStandEdgePoints()
        {
            Dictionary<string, Vector3> edges = new Dictionary<string, Vector3>();
            foreach (var obj in standEdgePoins)
            {
                Vector3 globalPosition = Model.transform.TransformPoint(obj.Value);
                edges.Add(obj.Key, globalPosition);
            }
            return edges;
        }

        public Dictionary<string, Vector3> GetStandFrontEdgePoins()
        {
            Dictionary<string, Vector3> edges = new Dictionary<string, Vector3>();
            foreach (var obj in standFrontEdgePoins)
            {
                Vector3 globalPosition = Model.transform.TransformPoint(obj.Value);
                edges.Add(obj.Key, globalPosition);
            }
            return edges;
        }

        public Dictionary<string, Vector3> GetClosestEdgesTo(GenericShip anotherShip)
        {
            KeyValuePair<string, Vector3> objThisNearest = new KeyValuePair<string, Vector3>("this", Vector3.zero);
            KeyValuePair<string, Vector3> objAnotherNearest = new KeyValuePair<string, Vector3>("another", Vector3.zero);
            float minDistance = float.MaxValue;
            foreach (var objThis in GetStandEdgePoints())
            {
                foreach (var objAnother in anotherShip.GetStandEdgePoints())
                {
                    float distance = Vector3.Distance(objThis.Value, objAnother.Value);
                    //Debug.Log ("Distance between " + objThis.Key + " and " + objAnother.Key + " is: " + distance.ToString ());
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        objThisNearest = objThis;
                        objAnotherNearest = objAnother;
                    }
                }
            }
            Dictionary<string, Vector3> result = new Dictionary<string, Vector3>
            {
                { "this", objThisNearest.Value },
                { "another", objAnotherNearest.Value }
            };
            return result;
        }

        public bool IsInside(Transform zone)
        {
            Vector3 zoneStart = zone.transform.TransformPoint(-0.5f, -0.5f, -0.5f);
            Vector3 zoneEnd = zone.transform.TransformPoint(0.5f, 0.5f, 0.5f);
            bool result = true;

            foreach (var point in GetStandEdgePoints())
            {
                
                if (point.Value.x < zoneStart.x) result = false;
                if (point.Value.z < zoneStart.z) result = false;
                if (point.Value.x > zoneEnd.x ) result = false;
                if (point.Value.z > zoneEnd.z) result = false;
            }
            return result;
        }

        public void ToggleDamaged(bool isDamaged)
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").Find("DamageParticles").gameObject.SetActive(isDamaged);
        }

        public void RotateModelDuringTurn(MovementExecutionData currentMovementData, MovementExecutionData previousMovementData)
        {
            if ((currentMovementData.MovementDirection == ManeuverDirection.Forward) && (previousMovementData.Speed == 0)) return;

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
            //Todo: Work correctly with move revertion
            //Todo: reset rotation on finish

            float progress = progressCurrent / progressTarget;
            if (progress > 0.5f)
            {
                progress = 1 - progress;
            }
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 45* turningDirection, progress));
        }

        public void RotateModelDuringBarrelRoll(float progress)
        {
            float turningDirection = 1;
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, turningDirection * 360, progress));
        }

        public void MoveUpwards(float progress)
        {
            progress = (progress > 0.5f) ? 1 - progress : progress;
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").localPosition = new Vector3(0, 3.67f + 4f * progress, 0);
        }

        public Vector3 InverseTransformPoint(Vector3 point)
        {
            return Model.transform.InverseTransformPoint(point);
        }

    }

}
