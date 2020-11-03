using BoardTools;
using System;
using System.Collections.Generic;

namespace Abilities
{
    public class RangeToHostCondition : Condition
    {
        private int MinRange;
        private int MaxRange;

        public RangeToHostCondition(int minRange = 0, int maxRange = 3)
        {
            MinRange = minRange;
            MaxRange = maxRange;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null || args.ShipAbilityHost == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            DistanceInfo distInfo = new DistanceInfo(args.ShipAbilityHost, args.ShipToCheck);
            return distInfo.Range >= MinRange && distInfo.Range <= MaxRange;
        }
    }
}
