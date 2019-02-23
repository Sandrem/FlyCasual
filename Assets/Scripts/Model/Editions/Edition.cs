using ActionsList;
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

namespace Editions
{
    public abstract class Edition
    {
        public static Edition Current { get; set; }

        public abstract string Name { get; }
        public abstract string NameShort { get; }
        public abstract int MaxPoints { get; }
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
        public abstract Vector2 UpgradeCardSize { get; }
        public abstract Vector2 UpgradeCardCompactOffset { get; }
        public abstract Vector2 UpgradeCardCompactSize { get; }

        public virtual bool IsSquadBuilderLocked { get { return false; } }

        public abstract int MinShipCost(Faction faction);

        public virtual void ActionIsFailed(GenericShip ship, GenericAction action, bool overWrittenInstead = false, bool hasSecondChance = false)
        {
            if (!overWrittenInstead && !hasSecondChance) ship.RemoveAlreadyExecutedAction(action);
            action.RevertActionOnFail(hasSecondChance);
        }

        public Edition()
        {
            Current = this;
        }

        public abstract void EvadeDiceModification(DiceRoll diceRoll);
        public abstract bool IsWeaponHaveRangeBonus(IShipWeapon weapon);
        public abstract void SetShipBaseImage(GenericShip ship);
        public abstract void BarrelRollTemplatePlanning();
        public abstract void DecloakTemplatePlanning();
        public abstract void ReloadAction();
        public abstract bool ReinforceEffectCanBeUsed(ArcFacing facing);
        public abstract bool ReinforcePostCombatEffectCanBeUsed(ArcFacing facing);
        public abstract void TimedBombActivationTime(GenericShip ship);
        public abstract void SquadBuilderIsOpened();
        public abstract bool IsTokenCanBeDiscardedByJam(GenericToken token);
        public abstract string GetPilotImageUrl(GenericShip ship, string filename);
        public abstract string GetUpgradeImageUrl(GenericUpgrade upgrade);

        public virtual void AdaptShipToRules(GenericShip ship) { }
        public virtual void AdaptPilotToRules(GenericShip ship) { }
        public virtual void AdaptUpgradeToRules(GenericUpgrade upgrade) { }
        public virtual void AdaptArcsToRules(GenericShip ship) { }
        public virtual void RotateMobileFiringArc(ArcFacing facing) { }
        public virtual void RotateMobileFiringArcAlt(ArcFacing facing) { }
        public virtual void SubScribeToGenericShipEvents(GenericShip ship) { }
        public virtual void WhenIonized(GenericShip ship) { }
    }
}