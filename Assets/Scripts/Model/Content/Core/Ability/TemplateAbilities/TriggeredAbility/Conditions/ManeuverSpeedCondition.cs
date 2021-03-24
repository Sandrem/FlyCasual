using BoardTools;
using System;
using System.Collections.Generic;

namespace Abilities
{
    public class ManeuverSpeedCondition : Condition
    {
        private int MinSpeed;
        private int MaxSpeed;

        public ManeuverSpeedCondition(int minSpeed = 0, int maxSpeed = 3)
        {
            MinSpeed = minSpeed;
            MaxSpeed = maxSpeed;
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
                return args.ShipToCheck.AssignedManeuver.Speed >= MinSpeed && args.ShipToCheck.AssignedManeuver.Speed <= MaxSpeed;
            }
            else
            {
                return false;
            }
        }
    }
}
