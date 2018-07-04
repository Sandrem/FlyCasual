using Arcs;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
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
        public abstract Dictionary<string, string> PreGeneratedAiSquadrons { get; }
        public abstract string PathToSavedSquadrons { get; }
        public abstract string RootUrlForImages { get; }

        public virtual bool IsSquadBuilderLocked { get { return false; } }

        public virtual void ActionIsFailed(GenericShip ship, Type actionType)
        {
            ship.RemoveAlreadyExecutedAction(actionType);

            Phases.GoBack();
        }

        public RuleSet()
        {
            Instance = this;
        }

        public abstract void EvadeDiceModification(DiceRoll diceRoll);
        public abstract bool PilotIsAllowed(GenericShip ship);
        public abstract bool ShipIsAllowed(GenericShip ship);
        public abstract bool WeaponHasRangeBonus();
        public abstract void SetShipBaseImage(GenericShip ship);
        public abstract void BarrelRollTemplatePlanning();
        public abstract void ReloadAction();
        public abstract bool ReinforceEffectCanBeUsed(ArcFacing facing);
        public abstract bool ReinforcePostCombatEffectCanBeUsed(ArcFacing facing);
        public abstract void TimedBombActivationTime(GenericShip ship);
        public abstract void SquadBuilderIsOpened();
        public abstract bool IsTokenCanBeDiscardedByJam(GenericToken token);

        public virtual void AdaptShipToRules(GenericShip ship) { }
        public virtual void AdaptPilotToRules(GenericShip ship) { }
        public virtual void AdaptUpgradeToRules(GenericUpgrade upgrade) { }
        public virtual void AdaptArcsToRules(GenericShip ship) { }
        public virtual void RotateMobileFiringArc(ArcFacing facing) { }
        public virtual void ActivateGenericUpgradeAbility(GenericUpgrade upgrade) { }
        public virtual void SubScribeToGenericShipEvents(GenericShip ship) { }
        public virtual void WhenIonized(GenericShip ship) { }
    }
}