using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Movement;

namespace Ship
{
    public class ManeuverInfo
    {
        public ManeuverHolder Movement;
        public MovementComplexity Complexity;

        public ManeuverInfo(ManeuverSpeed speed, ManeuverDirection direction, ManeuverBearing bearing, MovementComplexity complexity)
        {
            Movement = new ManeuverHolder(speed, direction, bearing);
            Complexity = complexity;
        }
    }

    public class ShipDialInfo
    {
        public Dictionary<ManeuverHolder, MovementComplexity> PrintedDial { get; private set; }

        public ShipDialInfo(params ManeuverInfo[] printedManeuvers)
        {
            PrintedDial = new Dictionary<ManeuverHolder, MovementComplexity>();
            foreach (ManeuverInfo maneuver in printedManeuvers)
            {
                PrintedDial.Add(maneuver.Movement, maneuver.Complexity);
            }
        }

        public ShipDialInfo(JSONObject manuevers)
        {
            PrintedDial = new Dictionary<ManeuverHolder, MovementComplexity>();
            foreach (string maneuver in manuevers.keys)
            {
                string[] arrParameters = maneuver.Split('.');

                ManeuverSpeed speed = ManeuverSpeed.Speed1;

                switch (arrParameters[0])
                {
                    case "0":
                        speed = ManeuverSpeed.Speed0;
                        break;
                    case "1":
                        speed = ManeuverSpeed.Speed1;
                        break;
                    case "2":
                        speed = ManeuverSpeed.Speed2;
                        break;
                    case "3":
                        speed = ManeuverSpeed.Speed3;
                        break;
                    case "4":
                        speed = ManeuverSpeed.Speed4;
                        break;
                    case "5":
                        speed = ManeuverSpeed.Speed5;
                        break;
                }

                ManeuverDirection direction = ManeuverDirection.Forward;

                switch (arrParameters[1])
                {
                    case "F":
                        direction = ManeuverDirection.Forward;
                        break;
                    case "L":
                        direction = ManeuverDirection.Left;
                        break;
                    case "R":
                        direction = ManeuverDirection.Right;
                        break;
                    case "S":
                        direction = ManeuverDirection.Stationary;
                        break;
                }

                ManeuverBearing bearing = ManeuverBearing.Straight;

                switch (arrParameters[2])
                {
                    case "S":
                        bearing = (speed != ManeuverSpeed.Speed0) ? ManeuverBearing.Straight : ManeuverBearing.Stationary;
                        break;
                    case "R":
                        bearing = (direction == ManeuverDirection.Forward) ? ManeuverBearing.KoiogranTurn : ManeuverBearing.SegnorsLoop;
                        break;
                    case "r":
                        bearing = ManeuverBearing.SegnorsLoopUsingTurnTemplate;
                        break;
                    case "E":
                        bearing = ManeuverBearing.TallonRoll;
                        break;
                    case "B":
                        bearing = ManeuverBearing.Bank;
                        break;
                    case "T":
                        bearing = ManeuverBearing.Turn;
                        break;
                    case "V":
                        bearing = ManeuverBearing.Reverse;
                        break;
                }

                MovementComplexity complexity = MovementComplexity.Normal;
                switch (manuevers[maneuver].str)                {                    case "Complex":                        complexity = MovementComplexity.Complex;                        break;                    case "Easy":                        complexity = MovementComplexity.Easy;                        break;                    default:                        complexity = MovementComplexity.Normal;                        break;                }

                PrintedDial.Add(new ManeuverHolder(speed, direction, bearing, complexity), complexity);
            }
        }

        public void ChangeManeuverComplexity(ManeuverHolder maneuver, MovementComplexity complexity)
        {
            ManeuverHolder match = PrintedDial.First(m => m.Key.Speed == maneuver.Speed && m.Key.Direction == maneuver.Direction && m.Key.Bearing == maneuver.Bearing).Key;
            PrintedDial[match] = complexity;
        }

        public void RemoveManeuver(ManeuverHolder maneuver)
        {
            ManeuverHolder match = PrintedDial.First(m => m.Key.Speed == maneuver.Speed && m.Key.Direction == maneuver.Direction && m.Key.Bearing == maneuver.Bearing).Key;
            PrintedDial.Remove(match);
        }

        public void AddManeuver(ManeuverHolder maneuver, MovementComplexity complexity)
        {
            PrintedDial.Add(maneuver, complexity);
        }
    }
}
