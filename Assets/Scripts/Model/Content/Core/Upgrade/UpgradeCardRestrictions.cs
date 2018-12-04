using Ship;
using System.Collections.Generic;
using System.Linq;
using Actions;
using Arcs;
using System;

namespace Upgrade
{
    public abstract class UpgradeCardRestriction
    {
        public abstract bool IsAllowedForShip(GenericShip ship);
    }

    public class FactionRestriction: UpgradeCardRestriction
    {
        public List<Faction> Factions { get; private set; }

        public FactionRestriction(params Faction[] factions)
        {
            Factions = factions.ToList();
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return Factions.Contains(ship.Faction);
        }
    }

    public class BaseSizeRestriction : UpgradeCardRestriction
    {
        public List<BaseSize> BaseSizes { get; private set; }

        public BaseSizeRestriction(params BaseSize[] baseSizes)
        {
            BaseSizes = baseSizes.ToList();
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return BaseSizes.Contains(ship.ShipInfo.BaseSize);
        }
    }

    
    public class UpgradeBarRestriction : UpgradeCardRestriction
    {
        public List<UpgradeType> UpgradeSlots { get; private set; }

        public UpgradeBarRestriction(params UpgradeType[] upgradeSlots)
        {
            UpgradeSlots = upgradeSlots.ToList();
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            // TODO
            return true;
        }
    }

    public class ActionBarRestriction : UpgradeCardRestriction
    {
        public ActionInfo Action { get; private set; }

        public ActionBarRestriction(ActionInfo action)
        {
            Action = action;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipInfo.ActionIcons.Actions.Any(a => a.ActionType == Action.ActionType && (Action.Color == ActionColor.White || a.Color == Action.Color));
        }
    }

    public class ShipRestriction : UpgradeCardRestriction
    {
        public List<Type> ShipTypes { get; private set; }

        public ShipRestriction(params Type[] shipTypes)
        {
            ShipTypes = shipTypes.ToList();
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ShipTypes.Any(n => ship.GetType().IsSubclassOf(n));
        }
    }

    public class ArcRestriction : UpgradeCardRestriction
    {
        public ArcType ArcType { get; private set; }

        public ArcRestriction(ArcType arcType)
        {
            ArcType = arcType;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            // TODO
            return true;
        }
    }

    public class UpgradeCardRestrictions
    {
        public List<UpgradeCardRestriction> Restrictions { get; private set; }

        public UpgradeCardRestrictions(params UpgradeCardRestriction[] restrictions)
        {
            Restrictions = restrictions.ToList();
        }

        public bool IsAllowedForShip(GenericShip ship)
        {
            bool result = true;

            foreach (UpgradeCardRestriction restriction in Restrictions)
            {
                if (!restriction.IsAllowedForShip(ship)) return false;
            }

            return result;
        }
    }
}
