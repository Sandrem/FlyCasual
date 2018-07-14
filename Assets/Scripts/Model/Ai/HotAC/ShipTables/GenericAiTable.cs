using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class GenericAiTable
    {
        private bool inDebug = false;

        protected List<string> FrontManeuversInner = new List<string>();
        protected List<string> FrontManeuversOuter = new List<string>();
        protected List<string> FrontSideManeuversInner = new List<string>();
        protected List<string> FrontSideManeuversOuter = new List<string>();
        protected List<string> SideManeuversInner = new List<string>();
        protected List<string> SideManeuversOuter = new List<string>();
        protected List<string> BackSideManeuversInner = new List<string>();
        protected List<string> BackSideManeuversOuter = new List<string>();
        protected List<string> BackManeuversInner = new List<string>();
        protected List<string> BackManeuversOuter = new List<string>();

        public GenericAiTable()
        {

        }

        public Movement.GenericMovement GetManeuver(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
        {
            float vector = Actions.GetVector(thisShip, anotherShip);
            bool isClosing = Actions.IsClosing(thisShip, anotherShip);
            if (inDebug) Debug.Log("Vector: " + vector + ", Closing: " + isClosing);
            Movement.GenericMovement result = GetManeuverFromTable(vector, isClosing);
            return result;
        }

        public Movement.GenericMovement GetManeuverFromTable(float vector, bool isClosing)
        {
            List<string> table = null;
            bool adjustDirection = false;

            if (isClosing)
            {
                if ((vector > -22.5f) && (vector < 22.5f))
                {
                    if (inDebug) Debug.Log("FrontManeuversInner");
                    table = FrontManeuversInner;
                }
                else if (((vector >= 22.5f) && (vector < 67.5f)) || ((vector <= -22.5f) && (vector > -67.5f)))
                {
                    if (inDebug) Debug.Log("FrontSideManeuversInner");
                    table = FrontSideManeuversInner;
                    adjustDirection = true;
                }
                else if (((vector >= 67.5f) && (vector < 112.5f)) || ((vector <= -67.5f) && (vector > -112.5f)))
                {
                    if (inDebug) Debug.Log("SideManeuversInner");
                    table = SideManeuversInner;
                    adjustDirection = true;
                }
                else if (((vector >= 112.5f) && (vector < 157.5f)) || ((vector <= -112.5f) && (vector > -157.5f)))
                {
                    if (inDebug) Debug.Log("BackSideManeuversInner");
                    table = BackSideManeuversInner;
                    adjustDirection = true;
                }
                else if ((vector >= 157.5f) || (vector <= -157.5f))
                {
                    if (inDebug) Debug.Log("BackManeuversInner");
                    table = BackManeuversInner;
                }
            }
            else
            {
                if ((vector > -22.5f) && (vector < 22.5f))
                {
                    if (inDebug) Debug.Log("FrontManeuversOuter");
                    table = FrontManeuversOuter;
                }
                else if (((vector >= 22.5f) && (vector < 67.5f)) || ((vector <= -22.5f) && (vector > -67.5f)))
                {
                    if (inDebug) Debug.Log("FrontSideManeuversOuter");
                    table = FrontSideManeuversOuter;
                    adjustDirection = true;
                }
                else if (((vector >= 67.5f) && (vector < 112.5f)) || ((vector <= -67.5f) && (vector > -112.5f)))
                {
                    if (inDebug) Debug.Log("SideManeuversOuter");
                    table = SideManeuversOuter;
                    adjustDirection = true;
                }
                else if (((vector >= 112.5f) && (vector < 157.5f)) || ((vector <= -112.5f) && (vector > -157.5f)))
                {
                    if (inDebug) Debug.Log("BackSideManeuversOuter");
                    table = BackSideManeuversOuter;
                    adjustDirection = true;
                }
                else if ((vector >= 157.5f) || (vector <= -157.5f))
                {
                    if (inDebug) Debug.Log("BackManeuversOuter");
                    table = BackManeuversOuter;
                }
            }

            Movement.GenericMovement result = RandomManeuverFromTable(table);
            if (adjustDirection)
            {
                if (inDebug) Debug.Log("Adjust direction according to vector: " + vector);
                result = AdjustDirection(result, vector);
            }


            return result;
        }

        private Movement.GenericMovement AdjustDirection(Movement.GenericMovement movement, float vector)
        {
            Movement.GenericMovement result = movement;
            if (movement.Direction != Movement.ManeuverDirection.Forward)
            {
                if (vector < 0)
                {
                    if (movement.Direction == Movement.ManeuverDirection.Left) movement.Direction = Movement.ManeuverDirection.Right;
                    else if (movement.Direction == Movement.ManeuverDirection.Right) movement.Direction = Movement.ManeuverDirection.Left;
                }
            }
            return result;
        }

        public Movement.GenericMovement RandomManeuverFromTable(List<string> table)
        {
            string result = "";
            int random = Random.Range(0, 6);
            if (inDebug) Debug.Log("Random is: " + random);
            result = table[random];

            if (inDebug) Debug.Log("Result is: " + result);

            return ShipMovementScript.MovementFromString(result);
        }

        public static bool IsClosing(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
        {
            bool result = false;
            float distanceToFront = Vector3.Distance(thisShip.GetPosition(), anotherShip.ShipBase.GetCentralFrontPoint());
            float distanceToBack = Vector3.Distance(thisShip.GetPosition(), anotherShip.ShipBase.GetCentralBackPoint());
            result = (distanceToFront < distanceToBack) ? true : false;
            return result;
        }

        public void Check(Dictionary<string, Movement.MovementComplexity> maneuvers)
        {

            foreach (var maneuver in FrontManeuversInner)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

            foreach (var maneuver in FrontManeuversOuter)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

            foreach (var maneuver in FrontSideManeuversInner)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

            foreach (var maneuver in FrontSideManeuversOuter)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

            foreach (var maneuver in SideManeuversInner)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

            foreach (var maneuver in SideManeuversOuter)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

            foreach (var maneuver in BackSideManeuversInner)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

            foreach (var maneuver in BackSideManeuversOuter)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

            foreach (var maneuver in BackManeuversInner)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

            foreach (var maneuver in BackManeuversOuter)
            {
                if (!maneuvers.ContainsKey(maneuver)) Debug.Log(this.ToString() + " has incorrect maneuver: " + maneuver);
            }

        }

    }
}