using Arcs;
using BoardTools;
using System;
using System.Collections.Generic;

namespace Abilities
{
    public class DefenderInSectorCondition : Condition
    {
        private ArcType ArcType;

        public DefenderInSectorCondition(ArcType arcType)
        {
            ArcType = arcType;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            return args.ShipToCheck.SectorsInfo.IsShipInSector(Combat.Defender, ArcType);
        }
    }
}
