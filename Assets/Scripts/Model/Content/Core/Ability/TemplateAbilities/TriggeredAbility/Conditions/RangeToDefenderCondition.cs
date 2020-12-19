using BoardTools;
using System;
using System.Collections.Generic;

namespace Abilities
{
    public class RangeToDefenderCondition : Condition
    {
        private int MinRange;
        private int MaxRange;

        public RangeToDefenderCondition(int minRange = 0, int maxRange = 3)
        {
            MinRange = minRange;
            MaxRange = maxRange;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null || Combat.Defender == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            DistanceInfo distInfo = new DistanceInfo(Combat.Defender, args.ShipToCheck);
            return distInfo.Range >= MinRange && distInfo.Range <= MaxRange;
        }
    }
}
