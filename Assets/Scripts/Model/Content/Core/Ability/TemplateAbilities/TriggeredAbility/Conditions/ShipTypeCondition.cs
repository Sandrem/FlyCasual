using System;
using System.Collections.Generic;

namespace Abilities
{
    public class ShipTypeCondition : Condition
    {
        private Type[] ShipTypes;

        public ShipTypeCondition(params Type[] shipTypes)
        {
            ShipTypes = shipTypes;
        }

        public override bool Passed(ConditionArgs args)
        {
            if (args.ShipToCheck == null)
            {
                Messages.ShowError("Ability Condition Error: ship is not set");
                return false;
            }

            foreach (Type shipType in ShipTypes)
            {
                if (args.ShipToCheck.GetType().IsSubclassOf(shipType)) return true;
            }

            return false;
        }
    }
}
