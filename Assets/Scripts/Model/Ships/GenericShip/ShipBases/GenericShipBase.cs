using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ship
{
    public enum BaseSize
    {
        Small,
        Large
    }

    public class GenericShipBase
    {
        public GenericShip Host { get; protected set; }
        public BaseSize Size { get; protected set; }
        public string PrefabPath { get; protected set; }
        public string TemporaryPrefabPath { get; protected set; }

        public Dictionary<string, Vector3> BaseEdges { get; private set; }

        protected Dictionary<string, Vector3> standFrontEdgePoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standFrontPoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standBackPoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standLeftPoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standRightPoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standFront180Points = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standBullseyePoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standEdgePoints = new Dictionary<string, Vector3>();
        private Dictionary<string, Vector3> standPoints = new Dictionary<string, Vector3>();
        public float HALF_OF_SHIPSTAND_SIZE { get; protected set; }
        public float SHIPSTAND_SIZE { get; protected set; }
        public float SHIPSTAND_SIZE_CM { get; protected set; }
        public float HALF_OF_FIRINGARC_SIZE { get; protected set; }
        private float BULLSEYE_ARC_PERCENT = 0.5f;

        public GenericShipBase(GenericShip host)
        {
            Host = host;
        }

        protected virtual void CreateShipBase()
        {
            GameObject prefab = (GameObject)Resources.Load(PrefabPath, typeof(GameObject));
            GameObject shipBase = MonoBehaviour.Instantiate(
                prefab,
                Host.Model.transform.position,
                Host.Model.transform.rotation,
                Host.GetShipAllPartsTransform()
            );
            shipBase.transform.localEulerAngles = shipBase.transform.localEulerAngles + new Vector3(0, 180, 0);
            shipBase.transform.localPosition = Vector3.zero;
            shipBase.name = "ShipBase";

            SetShipBaseEdges();
        }

        private void SetShipBaseEdges()
        {
            int PRECISION = 20;

            BaseEdges.Add("LF", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, 0f));
            BaseEdges.Add("RF", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, 0f));
            BaseEdges.Add("LB", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            BaseEdges.Add("RB", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));

            standEdgePoints.Add("LF", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, 0f));
            standEdgePoints.Add("CF", Vector3.zero);
            standEdgePoints.Add("RF", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, 0f));
            standEdgePoints.Add("LB", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoints.Add("CB", new Vector3(0f, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            standEdgePoints.Add("RB", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));

            standPoints = new Dictionary<string, Vector3>(standEdgePoints);

            standFrontEdgePoints.Add("LF", new Vector3(-HALF_OF_FIRINGARC_SIZE, 0f, 0f));
            standFrontEdgePoints.Add("CF", Vector3.zero);
            standFrontEdgePoints.Add("RF", new Vector3(HALF_OF_FIRINGARC_SIZE, 0f, 0f));

            standFrontPoints = new Dictionary<string, Vector3>(standFrontEdgePoints);
            for (int i = 1; i < PRECISION + 1; i++)
            {
                Vector3 newPoint = new Vector3((float)i * ((2 * HALF_OF_FIRINGARC_SIZE) / (float)(PRECISION + 1)) - HALF_OF_FIRINGARC_SIZE, 0f, 0f);
                standFrontPoints.Add("F" + i, newPoint);

                float frontPercent = (float)i / (float)PRECISION;
                if (InBullseyeArc(frontPercent)) standBullseyePoints.Add("F" + i, newPoint);

                standPoints.Add("F" + i, newPoint);
            }

            standFront180Points = new Dictionary<string, Vector3>(standFrontPoints);

            standBackPoints = new Dictionary<string, Vector3>();
            standBackPoints.Add("LB", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            standBackPoints.Add("RB", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            for (int i = 1; i < PRECISION + 1; i++)
            {
                Vector3 newPoint = new Vector3((float)i * ((2 * HALF_OF_FIRINGARC_SIZE) / (float)(PRECISION + 1)) - HALF_OF_FIRINGARC_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE);
                standBackPoints.Add("B" + i, newPoint);
                standPoints.Add("B" + i, newPoint);
            }

            standLeftPoints = new Dictionary<string, Vector3>();
            standLeftPoints.Add("LF", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, 0f));
            standLeftPoints.Add("LB", new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            for (int i = 1; i < PRECISION + 1; i++)
            {
                Vector3 newPoint = new Vector3(-HALF_OF_SHIPSTAND_SIZE, 0f, -(float)i * ((2 * HALF_OF_SHIPSTAND_SIZE) / (float)(PRECISION + 1)));
                standLeftPoints.Add("L" + i, newPoint);
                standPoints.Add("L" + i, newPoint);
                if (i <= (PRECISION / 2) + 1)
                {
                    standFront180Points.Add("L" + i, newPoint);
                }
            }

            standRightPoints = new Dictionary<string, Vector3>();
            standRightPoints.Add("LF", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, 0f));
            standRightPoints.Add("LB", new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, -2 * HALF_OF_SHIPSTAND_SIZE));
            for (int i = 1; i < PRECISION + 1; i++)
            {
                Vector3 newPoint = new Vector3(HALF_OF_SHIPSTAND_SIZE, 0f, -(float)i * ((2 * HALF_OF_SHIPSTAND_SIZE) / (float)(PRECISION + 1)));
                standRightPoints.Add("R" + i, newPoint);
                standPoints.Add("R" + i, newPoint);
                if (i <= (PRECISION / 2) + 1)
                {
                    standFront180Points.Add("R" + i, newPoint);
                }
            }
        }

        private bool InBullseyeArc(float frontPercent)
        {
            float minPercent = (1f - BULLSEYE_ARC_PERCENT) / 2f;
            float maxPercent = minPercent + BULLSEYE_ARC_PERCENT;
            return ((frontPercent >= minPercent) && (frontPercent <= maxPercent));
        }

        public Vector3 GetCentralFrontPoint()
        {
            return Host.Model.transform.Find("RotationHelper").TransformPoint(standEdgePoints["CF"]);
        }

        public Vector3 GetCentralBackPoint()
        {
            return Host.Model.transform.Find("RotationHelper").TransformPoint(standEdgePoints["CB"]);
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

        public Dictionary<string, Vector3> GetStandBackPoints()
        {
            return GetPoints(standBackPoints);
        }

        public Dictionary<string, Vector3> GetStandLeftPoints()
        {
            return GetPoints(standLeftPoints);
        }

        public Dictionary<string, Vector3> GetStandRightPoints()
        {
            return GetPoints(standRightPoints);
        }

        public Dictionary<string, Vector3> GetStandFront180Points()
        {
            return GetPoints(standFront180Points);
        }

        public Dictionary<string, Vector3> GetStandBullseyePoints()
        {
            return GetPoints(standBullseyePoints);
        }

        private Dictionary<string, Vector3> GetPoints(Dictionary<string, Vector3> points)
        {
            Dictionary<string, Vector3> edges = new Dictionary<string, Vector3>();
            foreach (var obj in points)
            {
                Vector3 globalPosition = Host.Model.transform.TransformPoint(obj.Value);
                edges.Add(obj.Key, globalPosition);
            }
            return edges;
        }

        //TODO: Remove as old
        public bool IsInside(Transform zone)
        {
            Vector3 zoneStart = zone.transform.TransformPoint(-0.5f, -0.5f, -0.5f);
            Vector3 zoneEnd = zone.transform.TransformPoint(0.5f, 0.5f, 0.5f);
            bool result = true;

            foreach (var point in GetStandEdgePoints())
            {
                if ((point.Value.x < zoneStart.x) || (point.Value.z < zoneStart.z) || (point.Value.x > zoneEnd.x) || (point.Value.z > zoneEnd.z))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public Dictionary<string, float> GetBounds()
        {
            List<Vector3> edgesList = new List<Vector3>();
            edgesList.Add(Host.Model.transform.TransformPoint(standEdgePoints["RF"]));
            edgesList.Add(Host.Model.transform.TransformPoint(standEdgePoints["LF"]));
            edgesList.Add(Host.Model.transform.TransformPoint(standEdgePoints["RB"]));
            edgesList.Add(Host.Model.transform.TransformPoint(standEdgePoints["LB"]));

            Dictionary<string, float> bounds = new Dictionary<string, float>();
            bounds.Add("minX", Mathf.Min(edgesList[0].x, edgesList[1].x, edgesList[2].x, edgesList[3].x));
            bounds.Add("maxX", Mathf.Max(edgesList[0].x, edgesList[1].x, edgesList[2].x, edgesList[3].x));
            bounds.Add("minZ", Mathf.Min(edgesList[0].z, edgesList[1].z, edgesList[2].z, edgesList[3].z));
            bounds.Add("maxZ", Mathf.Max(edgesList[0].z, edgesList[1].z, edgesList[2].z, edgesList[3].z));

            return bounds;
        }

        public float GetShipBaseDistance()
        {
            float result = Board.BoardManager.GetBoard().TransformVector(new Vector3(SHIPSTAND_SIZE_CM, 0, 0)).x;
            return result;
        }

    }
}
