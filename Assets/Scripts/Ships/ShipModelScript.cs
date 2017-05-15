using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public class ShipModelScript
    {

        public GameManagerScript Game;

        private GenericShip Ship;

        public GameObject Model
        {
            get;
            set;
        }

        private Dictionary<string, Vector3> standFrontEdgePoins = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standEdgePoins = new Dictionary<string, Vector3>();
        private const float HALF_OF_SHIPSTAND_SIZE = 0.5f;
        private const float SHIPSTAND_SIZE = 1f;

        public ShipModelScript(GenericShip ship, Vector3 position)
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Ship = ship;
            Model = Game.ShipFactory.CreateShipModel(ship, position);

            standFrontEdgePoins.Add("LF", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, 0));
            standFrontEdgePoins.Add("RF", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, 0));

            standEdgePoins.Add("LF", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, 0));
            standEdgePoins.Add("CF", Vector3.zero);
            standEdgePoins.Add("RF", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, 0));
            standEdgePoins.Add("LB", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoins.Add("CB", new Vector3(0, 0, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoins.Add("RB", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
        }

        public void SetShipInstertImage()
        {
            string materialName = Ship.PilotName;
            materialName = materialName.Replace(' ', '_');
            materialName = materialName.Replace('"', '_');
            Debug.Log(materialName);
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

        public ShipStandColliderScript GetShipStandScript()
        {
            return Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").GetComponent<ShipStandColliderScript>();
        }

        public void ApplyShader(string type)
        {
            if (type == "default")
            {
                foreach (Transform transform in Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Ship.Type))
                {
                    transform.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                }
            }
            if (type == "selectedYellow")
            {
                foreach (Transform transform in Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Ship.Type))
                {
                    transform.GetComponent<Renderer>().material.shader = Shader.Find("Outlined/Silhouetted Diffuse");
                    transform.GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.yellow);
                }
            }
            if (type == "selectedRed")
            {
                foreach (Transform transform in Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Ship.Type))
                {
                    transform.GetComponent<Renderer>().material.shader = Shader.Find("Outlined/Silhouetted Diffuse");
                    transform.GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.red);
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
                foreach (var objAnother in anotherShip.Model.GetStandEdgePoints())
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

    }

}
