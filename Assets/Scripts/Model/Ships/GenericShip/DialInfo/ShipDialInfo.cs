using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Movement;

namespace Ship
{
    public class ShipDialInfo
    {
        public List<DialManeuverInfo> PrintedDial { get; private set; }

        public ShipDialInfo(params DialManeuverInfo[] printedManeuvers)
        {
            PrintedDial = printedManeuvers.ToList();
        }

        public void ChangeManeuverComplexity(DialManeuverInfo changedManeuver)
        {
            DialManeuverInfo match = PrintedDial.First(m => m.Speed == changedManeuver.Speed && m.Direction == changedManeuver.Direction && m.Bearing == changedManeuver.Bearing);
            RemoveManeuver(match);
            AddManeuver(changedManeuver);
        }

        public void RemoveManeuver(DialManeuverInfo maneuverInfo)
        {
            DialManeuverInfo match = PrintedDial.First(m => m.Speed == maneuverInfo.Speed && m.Direction == maneuverInfo.Direction && m.Bearing == maneuverInfo.Bearing && m.Complexity == maneuverInfo.Complexity);
            PrintedDial.Remove(match);
        }

        public void AddManeuver(DialManeuverInfo maneuverInfo)
        {
            PrintedDial.Add(maneuverInfo);
        }
    }
}
