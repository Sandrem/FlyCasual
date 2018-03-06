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
        public int Range { get; private set; }
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

        protected RangeHolder MinDistance;
        protected RangeHolder AltDistance1;
        protected RangeHolder AltDistance2;

        public int MinRange { get; protected set; }

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

            foreach (var ship1point in Ship1.ShipBase.BaseEdges)
            {
                foreach (var ship2point in Ship2.ShipBase.BaseEdges)
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


