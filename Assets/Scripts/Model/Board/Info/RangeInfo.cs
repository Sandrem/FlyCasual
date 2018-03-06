using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Board
{
    public class RangeHolder
    {
        public Vector3 Point1 { get; private set; }
        public Vector3 Point2 { get; private set; }
        public int Range { get { return Mathf.Max(1, Mathf.CeilToInt(Distance / BoardManager.DISTANCE_INTO_RANGE)); }}
        public float Distance { get; private set; }

        public RangeHolder(Vector3 point1, Vector3 point2)
        {
            Point1 = point1;
            Point2 = point2;

            CalculateDistance();
        }

        private void CalculateDistance()
        {
            Distance = Vector3.Distance(Point1, Point2);
        }
    }

    public class RangeInfo
    {
        public GenericShip Ship1 { get; protected set; }
        public GenericShip Ship2 { get; protected set; }

        public RangeHolder MinDistance { get; protected set; }
        public RangeHolder AltDistance1 { get; protected set; }
        public RangeHolder AltDistance2 { get; protected set; }

        public int Range { get { return MinDistance.Range; } }

        public RangeInfo(GenericShip ship1, GenericShip ship2)
        {
            Ship1 = ship1;
            Ship2 = ship2;

            CheckRange();
        }

        private void CheckRange()
        {
            FindNearestDistances();
            TryFindPerpendicularDistance();
            CheckParallels();
        }

        private void FindNearestDistances()
        {
            List<RangeHolder> distances = new List<RangeHolder>();

            foreach (var ship1point in Ship1.ShipBase.GetBaseEdges())
            {
                foreach (var ship2point in Ship2.ShipBase.GetBaseEdges())
                {
                    distances.Add(new RangeHolder(ship1point.Value, ship2point.Value));
                }
            }

            distances = distances.OrderBy(n => n.Distance).ToList();

            MinDistance = distances.First();
            distances.Remove(MinDistance);

            AltDistance1 = distances.First(n => n.Point1 == MinDistance.Point1);
            AltDistance2 = distances.First(n => n.Point2 == MinDistance.Point2);
        }

        private void TryFindPerpendicularDistance()
        {
            float angleA = Vector3.Angle(MinDistance.Point2 - MinDistance.Point1, AltDistance1.Point2 - AltDistance1.Point1);
            Vector3 origVector = MinDistance.Point2 - MinDistance.Point1;
            float angleB = Vector3.Angle(MinDistance.Point2 - MinDistance.Point1, MinDistance.Point2 - AltDistance1.Point2);
            if (angleB >= 90 || angleA+angleB <= 90)
            {
                //Messages.ShowError("No, " + angleA + " " + angleB);
            }
            else
            {
                //Messages.ShowInfo("Yes, " + angleA + " " + angleB);
                float angleAnewRad = (180 - 90 - angleB) * Mathf.Deg2Rad;
                float distanceB = MinDistance.Distance * Mathf.Cos(angleAnewRad);
                float distanceA = distanceB * Mathf.Tan(angleAnewRad);

                Vector3 ship2sideVector = AltDistance1.Point2 - MinDistance.Point2;
                float scale = distanceA / Vector3.Distance(MinDistance.Point2, AltDistance1.Point2);

                Vector3 difference = ship2sideVector * scale;
                Vector3 nearestPoint = MinDistance.Point2 + difference;

                MinDistance = new RangeHolder(MinDistance.Point1, nearestPoint);
            }
            // Find Perpendiculars of two triangles.
            // Update MinDistance if needed
        }

        private void CheckParallels()
        {
            // If both alt distances ~same
            // find mid-points and set as min distance
        }
    }
}


