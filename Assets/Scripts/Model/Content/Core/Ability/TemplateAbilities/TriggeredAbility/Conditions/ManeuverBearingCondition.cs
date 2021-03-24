using Movement;
using System.Linq;

namespace Abilities
{
    public class ManeuverBearingCondition : Condition
    {
        private ManeuverBearing[] Bearings;

        public ManeuverBearingCondition(params ManeuverBearing[] bearings)
        {
            Bearings = bearings;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            if (args.ShipToCheck.AssignedManeuver != null)
            {
                return Bearings.ToList().Contains(args.ShipToCheck.AssignedManeuver.Bearing);
            }
            else
            {
                return false;
            }
        }
    }
}
