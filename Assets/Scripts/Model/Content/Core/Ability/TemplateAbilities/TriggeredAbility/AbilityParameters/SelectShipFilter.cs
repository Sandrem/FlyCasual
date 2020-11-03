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
        public List<Type> ShipTypesOnly { get; }

        public SelectShipFilter
        (
            int minRange = 0,
            int maxRange = int.MaxValue,
            ArcType inArcType = ArcType.None,
            TargetTypes targetTypes = TargetTypes.Any,
            Type hasToken = null,
            List<Type> shipTypesOnly = null
        )
        {
            MinRange = minRange;
            MaxRange = maxRange;
            InArcType = inArcType;
            TargetTypes = targetTypes;
            HasToken = hasToken;
            ShipTypesOnly = shipTypesOnly;
        }

        public bool FilterTargets(TriggeredAbility ability, GenericShip ship)
        {
            return ability.FilterTargetsByParameters(ship, MinRange, MaxRange, InArcType, TargetTypes, HasToken, ShipTypesOnly);
        }
    }
}
