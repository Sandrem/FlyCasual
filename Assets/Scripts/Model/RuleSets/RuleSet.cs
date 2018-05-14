using Arcs;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Upgrade;

namespace RuleSets
{
    public abstract class RuleSet
    {
        public static RuleSet Instance { get; private set; }

        public abstract string Name { get; }
        public abstract int MaxPoints { get; }
        public abstract int MaxShipsCount { get; }
        public abstract int MinShipsCount { get; }

        public RuleSet()
        {
            Instance = this;
        }

        public abstract void EvadeDiceModification(DiceRoll diceRoll);
        public abstract void ActionIsFailed(GenericShip ship, Type actionType);
        public abstract bool PilotIsAllowed(GenericShip ship);
        public abstract bool ShipIsAllowed(GenericShip ship);
        public abstract void AdaptShipToRules(GenericShip ship);
        public abstract void AdaptPilotToRules(GenericShip ship);
        public abstract void AdaptUpgradeToRules(GenericUpgrade upgrade);
        public abstract bool WeaponHasRangeBonus();
        public abstract void SetShipBaseImage(GenericShip ship);

        public virtual void RotateMobileFiringArc(ArcFacing facing) { }
        public virtual void ActivateGenericUpgradeAbility(GenericShip host, List<UpgradeType> upgradeTypes) { }
    }
}