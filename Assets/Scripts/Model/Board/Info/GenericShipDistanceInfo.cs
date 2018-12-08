﻿using RuleSets;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BoardTools
{
    public class RangeHolder
    {
        public Vector3 Point1 { get; private set; }
        public Vector3 Point2 { get; private set; }
        public GenericShip Ship1 { get; private set; }
        public GenericShip Ship2 { get; private set; }

        public float DistanceReal { get; private set; }
        public Vector3 Vector { get { return Point2 - Point1; } }

        public int Range
        {
            get
            {
                if (Edition.Current is SecondEdition)
                {
                    if (Ship1 == Ship2 || Ship1.ShipsBumped.Contains(Ship2))
                        return 0;
                }

                return Mathf.Max(1, Mathf.CeilToInt(DistanceReal / Board.DISTANCE_INTO_RANGE));
            }
        }

        public RangeHolder(Vector3 point1, Vector3 point2, GenericShip ship1, GenericShip ship2)
        {
            Point1 = point1;
            Point2 = point2;

            Ship1 = ship1;
            Ship2 = ship2;

            CalculateDistance();
        }

        private void CalculateDistance()
        {
            DistanceReal = Vector3.Distance(Point1, Point2);
        }
    }

    public class GenericShipDistanceInfo
    {
        public GenericShip Ship1 { get; protected set; }
        public GenericShip Ship2 { get; protected set; }

        public RangeHolder MinDistance { get; protected set; }
        private RangeHolder altDistance1;
        private RangeHolder altDistance2;
        private RangeHolder minDistancePerpA;
        private RangeHolder minDistancePerpB;

        public int Range { get { return MinDistance.Range; } }

        public GenericShipDistanceInfo(GenericShip ship1, GenericShip ship2)
        {
            Ship1 = ship1;
            Ship2 = ship2;
        }

        protected void FindNearestDistances(List<Vector3> firstShipEdges)
        {
            List<RangeHolder> distances = new List<RangeHolder>();

            // Check distances from all edges to all edges
            foreach (var ship1point in firstShipEdges)
            {
                foreach (var ship2point in Ship2.ShipBase.GetBaseEdges().Values.ToList())
                {
                    distances.Add(new RangeHolder(ship1point, ship2point, Ship1, Ship2));
                }
            }

            distances = distances.OrderBy(n => n.DistanceReal).ToList();

            // MinDistance - shortest distance between edges
            MinDistance = distances.First();
            distances.Remove(MinDistance);

            // Save alternative short distances for perpendicular distance calculations
            altDistance1 = distances.First(n => n.Point1 == MinDistance.Point1);
            altDistance2 = distances.First(n => n.Point2 == MinDistance.Point2);
        }

        protected void TryFindPerpendicularDistanceA()
        {
            //Try to get perpendicular
            float angleA = Vector3.Angle(MinDistance.Point2 - MinDistance.Point1, altDistance1.Point2 - altDistance1.Point1);
            float angleB = Vector3.Angle(MinDistance.Point2 - MinDistance.Point1, MinDistance.Point2 - altDistance1.Point2);

            if (angleB >= 90 || angleA + angleB <= 90)
            {
                // No correct perpendicular for this triangle
            }
            else
            {
                // Correct perpendicular exists
                float angleAnewRad = (180 - 90 - angleB) * Mathf.Deg2Rad;
                float distanceB = MinDistance.DistanceReal * Mathf.Cos(angleAnewRad);
                float distanceA = distanceB * Mathf.Tan(angleAnewRad);

                Vector3 ship2sideVector = altDistance1.Point2 - MinDistance.Point2;
                float scale = distanceA / Vector3.Distance(MinDistance.Point2, altDistance1.Point2);

                Vector3 difference = ship2sideVector * scale;
                Vector3 nearestPoint = MinDistance.Point2 + difference;

                minDistancePerpA = new RangeHolder(MinDistance.Point1, nearestPoint, Ship1, Ship2);
            }
        }

        protected void TryFindPerpendicularDistanceB()
        {
            //Try to get perpendicular
            float angleA = Vector3.Angle(MinDistance.Point1 - MinDistance.Point2, altDistance2.Point1 - altDistance2.Point2);
            float angleB = Vector3.Angle(MinDistance.Point1 - MinDistance.Point2, MinDistance.Point1 - altDistance2.Point1);

            if (angleB >= 90 || angleA + angleB <= 90)
            {
                // No correct perpendicular for this triangle
            }
            else
            {
                // Correct perpendicular exists
                float angleAnewRad = (180 - 90 - angleB) * Mathf.Deg2Rad;
                float distanceB = MinDistance.DistanceReal * Mathf.Cos(angleAnewRad);
                float distanceA = distanceB * Mathf.Tan(angleAnewRad);

                Vector3 ship2sideVector = altDistance2.Point1 - MinDistance.Point1;
                float scale = distanceA / Vector3.Distance(MinDistance.Point1, altDistance2.Point1);

                Vector3 difference = ship2sideVector * scale;
                Vector3 nearestPoint = MinDistance.Point1 + difference;

                minDistancePerpB = new RangeHolder(nearestPoint, MinDistance.Point2, Ship1, Ship2);
            }
        }

        protected void SetFinalMinDistance()
        {
            if (minDistancePerpA != null) MinDistance = minDistancePerpA;
            if (minDistancePerpB != null && minDistancePerpB.DistanceReal < MinDistance.DistanceReal) MinDistance = minDistancePerpB;
        }
    }
}
