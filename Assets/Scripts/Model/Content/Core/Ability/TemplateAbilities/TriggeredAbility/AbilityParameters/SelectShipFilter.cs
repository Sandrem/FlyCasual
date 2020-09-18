using Arcs;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;

namespace Abilities.Parameters
{
    public class SelectShipFilter
    {
        public int MinRange { get; }
        public int MaxRange { get; }
        public ArcType InArcType { get; }
        public TargetTypes TargetTypes { get; }
        public Type HasToken { get; }

        public SelectShipFilter
        (
            int minRange,
            int maxRange,
            ArcType inArcType = ArcType.None,
            TargetTypes targetTypes = TargetTypes.Any,
            Type hasToken = null)
        {
            MinRange = minRange;
            MaxRange = maxRange;
            InArcType = inArcType;
            TargetTypes = targetTypes;
            HasToken = hasToken;
        }

        public bool FilterTargets(TriggeredAbility ability, GenericShip ship)
        {
            return ability.FilterTargetsByParameters(ship, MinRange, MaxRange, InArcType, TargetTypes, HasToken);
        }
    }
}
