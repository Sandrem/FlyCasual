using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {

        private Dictionary<string, Vector3> standFrontEdgePoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standFrontPoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standEdgePoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standPoints = new Dictionary<string, Vector3>();
        private const float HALF_OF_SHIPSTAND_SIZE = 0.5f;
        private const float SHIPSTAND_SIZE = 1f;

        private void SetShipBaseEdges()
        {
            int PRECISION = 20;

            standFrontEdgePoints.Add("LF", new Vector3(-HALF_OF_FIRINGARC_SIZE, 0f, 0f));
            standFrontEdgePoints.Add("CF", Vector3.zero);
            standFrontEdgePoints.Add("RF", new Vector3(HALF_OF_FIRINGARC_SIZE, 0f, 0f));

            standFrontPoints = new Dictionary<string, Vector3>(standFrontEdgePoints);
            for (int i = 1; i < PRECISION+1; i++)
            {
                standFrontPoints.Add("F" + i, new Vector3( (float)i*((2 * HALF_OF_FIRINGARC_SIZE) / (float)(PRECISION + 1)) - HALF_OF_FIRINGARC_SIZE, 0f, 0f));
            }

            standEdgePoints.Add("LF", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, 0f));
            standEdgePoints.Add("CF", Vector3.zero);
            standEdgePoints.Add("RF", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, 0f));
            standEdgePoints.Add("LB", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoints.Add("CB", new Vector3(0f, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoints.Add("RB", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));

            standPoints = new Dictionary<string, Vector3>(standEdgePoints);
            for (int i = 1; i < PRECISION+1; i++)
            {
                standPoints.Add("F" + i, new Vector3((float)i * ((2* HALF_OF_SHIPSTAND_SIZE) / (float)(PRECISION + 1)) - HALF_OF_SHIPSTAND_SIZE, 0f, 0f));
                standPoints.Add("B" + i, new Vector3((float)i * ((2* HALF_OF_SHIPSTAND_SIZE) / (float)(PRECISION + 1)) - HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
                standPoints.Add("L" + i, new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -(float)i * ((2* HALF_OF_SHIPSTAND_SIZE) / (float)(PRECISION + 1))));
                standPoints.Add("R" + i, new Vector3( HALF_OF_SHIPSTAND_SIZE, 0f, -(float)i * ((2* HALF_OF_SHIPSTAND_SIZE) / (float)(PRECISION + 1))));
            }
        }

        public Vector3 GetCentralFrontPoint()
        {
            return Model.transform.Find("RotationHelper").TransformPoint(standEdgePoints["CF"]);
        }

        public Vector3 GetCentralBackPoint()
        {
            return Model.transform.Find("RotationHelper").TransformPoint(standEdgePoints["CB"]);
        }

        public Dictionary<string, Vector3> GetStandEdgePoints()
        {
            return GetPoints(standEdgePoints);
        }

        public Dictionary<string, Vector3> GetStandFrontEdgePoins()
        {
            return GetPoints(standFrontEdgePoints);
        }

        public Dictionary<string, Vector3> GetStandPoints()
        {
            return GetPoints(standPoints);
        }

        public Dictionary<string, Vector3> GetStandFrontPoints()
        {
            return GetPoints(standFrontPoints);
        }

        private Dictionary<string, Vector3> GetPoints(Dictionary<string, Vector3> points)
        {
            Dictionary<string, Vector3> edges = new Dictionary<string, Vector3>();
            foreach (var obj in points)
            {
                Vector3 globalPosition = Model.transform.TransformPoint(obj.Value);
                edges.Add(obj.Key, globalPosition);
            }
            return edges;
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
