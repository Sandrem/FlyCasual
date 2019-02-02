using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Movement
{
    public enum ManeuverSpeed
    {
        AdditionalMovement,
        Speed0,
        Speed1,
        Speed2,
        Speed3,
        Speed4,
        Speed5
    }

    public enum ManeuverDirection
    {
        Left,
        Forward,
        Right,
        Stationary
    }

    public enum ManeuverBearing
    {
        Straight,
        Bank,
        Turn,
        KoiogranTurn,
        SegnorsLoop,
        TallonRoll,
        Stationary,
        Reverse,
        SegnorsLoopUsingTurnTemplate
    }

    public enum MovementComplexity
    {
        None,
        Easy,
        Normal,
        Complex
    }

    public struct ManeuverHolder
    {
        public ManeuverSpeed Speed;
        public ManeuverDirection Direction;
        public ManeuverBearing Bearing;
        public MovementComplexity ColorComplexity;

        private string shipTag;

        public ManeuverHolder(ManeuverSpeed speed, ManeuverDirection direction, ManeuverBearing bearing, MovementComplexity complexity = MovementComplexity.None)
        {
            Speed = speed;
            Direction = direction;
            Bearing = bearing;
            ColorComplexity = complexity;

            shipTag = null;
        }

        public ManeuverHolder(string parameters, Ship.GenericShip ship = null)
        {
            string[] arrParameters = parameters.Split('.');

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

            Speed = speed;
            Direction = direction;
            Bearing = bearing;

            ship = ship ?? Selection.ThisShip;
            shipTag = ship.GetTag();

            if (!ship.Maneuvers.ContainsKey(parameters))
            {
                Console.Write("<b>Ship " + ship.ShipInfo.ShipName + " doesn't have maneuver " + parameters + "</b>", LogTypes.Errors, true, "red");
            }
            ColorComplexity = ship.Maneuvers[parameters];
            ColorComplexity = ship.GetColorComplexityOfManeuver(this);
        }

        public void UpdateColorComplexity()
        {
            string parameters = this.ToString();

            Ship.GenericShip ship = Roster.GetShipById(shipTag) ?? Selection.ThisShip;
            if (!ship.Maneuvers.ContainsKey(parameters))
            {
                Console.Write(ship.ShipInfo.ShipName + " doesn't have " + parameters + " maneuver!", LogTypes.Errors, true, "red");
            }
            else
            {
                ColorComplexity = ship.Maneuvers[parameters];
                ColorComplexity = ship.GetColorComplexityOfManeuver(this);
            }
        }

        public int SpeedInt
        {
            set
            {
                ManeuverSpeed speed = ManeuverSpeed.Speed1;
                switch (value)
                {
                    case 0:
                        speed = ManeuverSpeed.Speed0;
                        break;
                    case 1:
                        speed = ManeuverSpeed.Speed1;
                        break;
                    case 2:
                        speed = ManeuverSpeed.Speed2;
                        break;
                    case 3:
                        speed = ManeuverSpeed.Speed3;
                        break;
                    case 4:
                        speed = ManeuverSpeed.Speed4;
                        break;
                    case 5:
                        speed = ManeuverSpeed.Speed5;
                        break;
                    default:
                        break;
                }
                Speed = speed;
            }

            get
            {
                int speed = 0;
                switch (Speed)
                {
                    case ManeuverSpeed.AdditionalMovement:
                        break;
                    case ManeuverSpeed.Speed0:
                        speed = 0;
                        break;
                    case ManeuverSpeed.Speed1:
                        speed = 1;
                        break;
                    case ManeuverSpeed.Speed2:
                        speed = 2;
                        break;
                    case ManeuverSpeed.Speed3:
                        speed = 3;
                        break;
                    case ManeuverSpeed.Speed4:
                        speed = 4;
                        break;
                    case ManeuverSpeed.Speed5:
                        speed = 5;
                        break;
                    default:
                        break;
                }
                return speed;
            }
        }

        public override string ToString()
        {
            string maneuverString = "";

            maneuverString += SpeedInt + ".";

            switch (Direction)
            {
                case ManeuverDirection.Left:
                    maneuverString += "L.";
                    break;
                case ManeuverDirection.Forward:
                    maneuverString += "F.";
                    break;
                case ManeuverDirection.Right:
                    maneuverString += "R.";
                    break;
                case ManeuverDirection.Stationary:
                    maneuverString += "S.";
                    break;
                default:
                    break;
            }

            switch (Bearing)
            {
                case ManeuverBearing.Straight:
                    maneuverString += "S";
                    break;
                case ManeuverBearing.Bank:
                    maneuverString += "B";
                    break;
                case ManeuverBearing.Turn:
                    maneuverString += "T";
                    break;
                case ManeuverBearing.KoiogranTurn:
                    maneuverString += "R";
                    break;
                case ManeuverBearing.SegnorsLoop:
                    maneuverString += "R";
                    break;
                case ManeuverBearing.SegnorsLoopUsingTurnTemplate:
                    maneuverString += "r";
                    break;
                case ManeuverBearing.TallonRoll:
                    maneuverString += "E";
                    break;
                case ManeuverBearing.Stationary:
                    maneuverString += "S";
                    break;
                case ManeuverBearing.Reverse:
                    maneuverString += "V";
                    break;
                default:
                    break;
            }

            return maneuverString;
        }
    }
}
