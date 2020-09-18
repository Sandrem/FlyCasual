using Arcs;
using Ship;
using System.Collections.Generic;

namespace Abilities.Parameters
{
    public class SelectShipFilter
    {
        public int MinRange { get; }
        public int MaxRange { get; }
        public ArcType InArcType { get; }

        public SelectShipFilter(int minRange, int maxRange, ArcType inArcType)
        {
            MinRange = minRange;
            MaxRange = maxRange;
            InArcType = inArcType;
        }

        public bool FilterTargets(TriggeredAbility ability, GenericShip ship)
        {
            return ability.FilterTargetsByRangeInSpecificArc(ship, MinRange, MaxRange, InArcType);
        }
    }
}
