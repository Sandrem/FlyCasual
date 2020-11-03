using System;
using System.Collections.Generic;

namespace Abilities
{
    public class ShipTypeCondition : Condition
    {
        private Type ShipType;

        public ShipTypeCondition(Type shipType)
        {
            ShipType = shipType;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            return args.ShipToCheck.GetType().IsSubclassOf(ShipType);
        }
    }
}
