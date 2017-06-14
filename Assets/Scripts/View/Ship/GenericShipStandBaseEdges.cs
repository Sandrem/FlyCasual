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

        private void setShipBaseEdges()
        {
            standFrontEdgePoins.Add("LF", new Vector3(-HALF_OF_FIRINGARC_SIZE, 0f, 0));
            standFrontEdgePoins.Add("CF", Vector3.zero);
            standFrontEdgePoins.Add("RF", new Vector3(HALF_OF_FIRINGARC_SIZE, 0f, 0));

            standEdgePoins.Add("LF", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, 0));
            standEdgePoins.Add("CF", Vector3.zero);
            standEdgePoins.Add("RF", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, 0));
            standEdgePoins.Add("LB", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoins.Add("CB", new Vector3(0, 0, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoins.Add("RB", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
        }

        public Vector3 GetCentralFrontPoint()
        {
            return Model.transform.Find("RotationHelper").TransformPoint(standEdgePoins["CF"]);
        }

        public Vector3 GetCentralBackPoint()
        {
            return Model.transform.Find("RotationHelper").TransformPoint(standEdgePoins["CB"]);
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

        //TODO: 2 same

        public Dictionary<string, Vector3> GetClosestEdgesTo(GenericShip anotherShip)
        {
            return GetClosestBetweenTwoPointDicts(GetStandEdgePoints(), anotherShip.GetStandEdgePoints());
        }

        public Dictionary<string, Vector3> GetClosestFiringEdgesTo(GenericShip anotherShip)
        {
            return GetClosestBetweenTwoPointDicts(GetStandFrontEdgePoins(), anotherShip.GetStandEdgePoints());
        }

        private Dictionary<string, Vector3> GetClosestBetweenTwoPointDicts(Dictionary<string, Vector3> dict1, Dictionary<string, Vector3> dict2)
        {
            KeyValuePair<string, Vector3> objThisNearest = new KeyValuePair<string, Vector3>("this", Vector3.zero);
            KeyValuePair<string, Vector3> objAnotherNearest = new KeyValuePair<string, Vector3>("another", Vector3.zero);
            float minDistance = float.MaxValue;
            foreach (var objThis in dict1)
            {
                foreach (var objAnother in dict2)
                {
                    float distance = Vector3.Distance(objThis.Value, objAnother.Value);
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
                if ((point.Value.x < zoneStart.x) || (point.Value.z < zoneStart.z) || (point.Value.x > zoneEnd.x ) || (point.Value.z > zoneEnd.z))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

    }

}
