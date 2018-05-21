using Arcs;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Upgrade;

namespace RuleSets
{
    public abstract class RuleSet
    {
        public static RuleSet Instance { get; private set; }

        public abstract string Name { get; }
        public abstract int MaxPoints { get; }
        public abstract int MinShipCost { get; }
        public abstract int MaxShipsCount { get; }
        public abstract int MinShipsCount { get; }
        public abstract string CombatPhaseName { get; }
        public abstract Color MovementEasyColor { get; }
        public abstract MovementComplexity IonManeuverComplexity { get; }
        public abstract Dictionary<Type, int> DamageDeckContent { get; }
        public abstract Dictionary<BaseSize, int> NegativeTokensToAffectShip { get; }

        public RuleSet()
        {
            Instance = this;
        }

        public abstract void EvadeDiceModification(DiceRoll diceRoll);
        public abstract void ActionIsFailed(GenericShip ship, Type actionType);
        public abstract bool PilotIsAllowed(GenericShip ship);
        public abstract bool ShipIsAllowed(GenericShip ship);
        public abstract bool WeaponHasRangeBonus();
        public abstract void SetShipBaseImage(GenericShip ship);
        public abstract void BarrelRollTemplatePlanning();
        public abstract void ReloadAction();

        public virtual void AdaptShipToRules(GenericShip ship) { }
        public virtual void AdaptPilotToRules(GenericShip ship) { }
        public virtual void AdaptUpgradeToRules(GenericUpgrade upgrade) { }
        public virtual void AdaptArcsToRules(GenericShip ship) { }
        public virtual void RotateMobileFiringArc(ArcFacing facing) { }
        public virtual void ActivateGenericUpgradeAbility(GenericShip host, List<UpgradeType> upgradeTypes) { }
        public virtual void SubScribeToGenericShipEvents(GenericShip ship) { }
    }
}