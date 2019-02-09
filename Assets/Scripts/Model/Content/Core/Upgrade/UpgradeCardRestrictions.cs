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
            foreach (UpgradeType upgrade in UpgradeSlots)
            {
                if (!ship.ShipInfo.UpgradeIcons.Upgrades.Contains(upgrade)) return false;
            }

            return true;
        }
    }

    public class ActionBarRestriction : UpgradeCardRestriction
    {
        public Type ActionType { get; private set; }
        public ActionColor? ActionColor { get; private set; }

        public ActionBarRestriction(Type actionType, ActionColor? color = null)
        {
            ActionType = actionType;
            ActionColor = color;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ShipInfo.ActionIcons.Actions.Any(a => 
                a.ActionType == ActionType
                && ((a.Color == ActionColor) || ActionColor == null)
            );
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
            return ship.ShipInfo.ArcInfo.Arcs.Any(a => a.ArcType == ArcType);
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
