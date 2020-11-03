using Ship;
using System;
using System.Collections.Generic;

namespace Abilities
{
    public class ConditionArgs
    {
        public GenericShip ShipToCheck { get; set; }
        public GenericShip ShipAbilityHost { get; set; }
    }

    public abstract class Condition
    {
        public abstract bool Passed(ConditionArgs args);
    }
}
