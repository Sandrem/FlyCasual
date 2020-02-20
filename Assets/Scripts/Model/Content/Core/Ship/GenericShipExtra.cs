using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mods;
using ActionsList;
using Upgrade;
using Editions;
using SquadBuilderNS;
using Tokens;
using System.Linq;

namespace Ship
{
    public enum WingsPositions { Opened, Closed, None };

    public interface IMovableWings
    {
        WingsPositions CurrentWingsPosition { get; set; }
    }

    public partial class GenericShip
    {
        public Dictionary<Faction, Type> IconicPilots;
        public ShipSoundInfo SoundInfo;

        public event EventHandlerShip OnDocked;
        public event EventHandlerShip OnUndocked;
        public event EventHandlerShip OnUndockingFinish;
        public event EventHandlerShip OnAnotherShipDocked;
        public event EventHandlerShip OnAnotherShipUndocked;

        public event EventHandlerShip OnShipIsPlaced;
        public event EventHandler OnGameStart;

        public event EventHandlerActionInt OnAiGetDiceModificationPriority;

        public event EventHandlerBool OnCanReleaseDockedShipRegular;
        public event EventHandlerBoolDirection OnOffTheBoard;

        public event EventHandlerShip OnSetupSelected;
        public static event EventHandlerShip OnSetupSelectedGlobal;
        public event EventHandlerShip OnSetupPlaced;
        public static event EventHandlerShip OnSetupPlacedGlobal;

        public event EventHandlerShipRefBool OnBullseyeArcCheck;

        public event EventHandlerShipRefBool OnTryAttackSameTeamCheck;

        public event EventHandlerShipRefVector OnGetDockingRange;

        public event EventHandlerBool OnSelectDamageCardToExpose;
        public event EventHandlerDamageCard OnFaceupDamageCardIsRepaired;

        public GenericShip DockingHost;

        public Type ShipRuleType = typeof(Editions.FirstEdition);
        public Type PilotRuleType = typeof(Editions.FirstEdition);

        public List<GenericShip> TwoTargetLocksOnDifferentTargetsAreAllowed = new List<GenericShip>();
        public List<GenericShip> TwoTargetLocksOnSameTargetsAreAllowed = new List<GenericShip>();


        private string imageUrl;
        public string ImageUrl
        {
            get
            {
                return imageUrl ?? ImageUrls.GetImageUrl(this);
            }
            set
            {
                imageUrl = value;
            }
        }

        public string ManeuversImageUrl { get; protected set; }

        public bool IsHidden { get; protected set; }

        public bool IsStressed { get { return Tokens.HasToken<Tokens.StressToken>(); } }

        public bool IsTractored
        {
            get
            {
                return Tokens.CountTokensByType<TractorBeamToken>() >= Edition.Current.NegativeTokensToAffectShip[ShipBase.Size];
            }
        }

        public bool IsAttacking { get { return Combat.AttackStep == CombatStep.Attack && Combat.Attacker == this; } }

        public bool IsDefending { get { return Combat.AttackStep == CombatStep.Defence && Combat.Defender == this; } }

        public char ShipIconLetter { get; protected set; }

        public List<Type> RequiredMods { get; set; }

        public event EventHandler OnDiscardUpgrade;
        public static EventHandler OnDiscardUpgradeGlobal;

        public event EventHandlerUpgrade OnAfterDiscardUpgrade;

        public event EventHandler OnFlipFaceUpUpgrade;
        public event EventHandlerUpgrade OnAfterFlipFaceUpUpgrade;

        public event EventHandlerUpgrade OnPreInstallUpgrade;
        public event EventHandlerUpgrade OnRemovePreInstallUpgrade;

        public event EventHandlerDualUpgrade OnAfterDualCardSideSelected;

        public event EventHandlerShip OnSystemsAbilityActivation;
        public event EventHandlerShipRefBool OnCheckSystemsAbilityActivation;

        public event EventHandlerCheckRange OnCheckRange;

        public Func<Direction, bool> FilterUndockDirection { get; set; } = delegate { return true; };

        public bool IsStrained
        {
            get { return Tokens.HasToken<StrainToken>(); }
        }

        public bool IsDepleted
        {
            get { return Tokens.HasToken<DepleteToken>(); }
        }

        public virtual bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            return true;
        }

        public void CallOnGameStart()
        {
            if (OnGameStart != null) OnGameStart();
        }

        public bool CheckSystemsAbilityActivation()
        {
            bool hasSystemActivation = false;
            OnCheckSystemsAbilityActivation?.Invoke(this, ref hasSystemActivation);
            return hasSystemActivation;
        }

        public void CallOnSystemsPhaseActivation(Action callback)
        {
            this.IsSystemsAbilityInactive = true;
            OnSystemsAbilityActivation?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnSystemsAbilityActivation, callback);
        }

        public void CallOnShipIsPlaced(Action callback)
        {
            if (OnShipIsPlaced != null) OnShipIsPlaced(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipIsPlaced, callback);
        }

        public void CallDiscardUpgrade(Action callBack)
        {
            if (OnDiscardUpgrade != null) OnDiscardUpgrade();
            if (OnDiscardUpgradeGlobal != null) OnDiscardUpgradeGlobal();

            Triggers.ResolveTriggers(TriggerTypes.OnDiscard, callBack);
        }

        public void CallFlipFaceUpUpgrade(Action callBack)
        {
            if (OnFlipFaceUpUpgrade != null) OnFlipFaceUpUpgrade();

            Triggers.ResolveTriggers(TriggerTypes.OnFlipFaceUp, callBack);
        }

        public void CallAfterDiscardUpgrade(GenericUpgrade discardedUpgrade, Action callBack)
        {
            if (OnAfterDiscardUpgrade != null) OnAfterDiscardUpgrade(discardedUpgrade);

            Triggers.ResolveTriggers(TriggerTypes.OnAfterDiscard, callBack);
        }

        public void CallAfterFlipFaceUpUpgrade(GenericUpgrade flippedFaceUpUpgrade, Action callBack)
        {
            if (OnAfterFlipFaceUpUpgrade != null) OnAfterFlipFaceUpUpgrade(flippedFaceUpUpgrade);

            Triggers.ResolveTriggers(TriggerTypes.OnAfterFlipFaceUp, callBack);
        }

        public void CallOnAfterDualUpgradeSideSelected(GenericDualUpgrade upgrade)
        {
            if (OnAfterDualCardSideSelected != null) OnAfterDualCardSideSelected(upgrade);
        }

        public List<GenericShip> DockedShips = new List<GenericShip>();

        public void ToggleDockedModel(GenericShip dockedShip, bool isVisible)
        {
            GetModelTransform().Find("DockedShips").transform.Find(dockedShip.ModelInfo.ModelName).gameObject.SetActive(isVisible);
        }

        public void CallDocked(GenericShip hostShip)
        {
            OnDocked?.Invoke(hostShip);
        }

        public void CallUndocked(GenericShip host)
        {
            OnUndocked?.Invoke(host);
        }

        public void CallUndockingFinish(GenericShip host, Action callback)
        {
            OnUndockingFinish?.Invoke(host);

            Triggers.ResolveTriggers(TriggerTypes.OnUndockingFinish, callback);
        }

        public void CallAnotherShipDocked(GenericShip dockedShip)
        {
            OnAnotherShipDocked?.Invoke(dockedShip);
        }

        public void CallAnotherShipUndocked(GenericShip dockedShip)
        {
            OnAnotherShipUndocked?.Invoke(dockedShip);
        }

        public virtual bool IsAllowedForSquadBuilder()
        {
            bool result = true;

            if (IsHidden) return false;

            if (RequiredMods.Count != 0)
            {
                foreach (var modType in RequiredMods)
                {
                    if (!ModsManager.Mods[modType].IsAvailable()) return false;
                }
            }

            return result;
        }

        public void CheckAITable()
        {
            if (HotacManeuverTable != null)
            {
                HotacManeuverTable.Check(this.Maneuvers);
            }
        }

        public void SetDockedName(bool isActive)
        {
            string dockedPosfix = " (Docked)";
            if (isActive)
            {
                PilotName = PilotName + dockedPosfix;
            }
            else
            {
                PilotName = PilotName.Replace(dockedPosfix, "");
            }
            Roster.UpdateShipStats(this);
        }

        public void SetInReserveName(bool isActive)
        {
            string dockedPosfix = " (In Reserve)";
            if (isActive)
            {
                PilotName = PilotName + dockedPosfix;
            }
            else
            {
                PilotName = PilotName.Replace(dockedPosfix, "");
            }
            Roster.UpdateShipStats(this);
        }

        // AI

        public void CallOnAiGetDiceModificationPriority(GenericAction diceModification, ref int priority)
        {
            if (OnAiGetDiceModificationPriority != null) OnAiGetDiceModificationPriority(diceModification, ref priority);
        }

        public bool CheckCanReleaseDockedShipRegular()
        {
            bool canRelease = true;
            OnCanReleaseDockedShipRegular?.Invoke(ref canRelease);
            return canRelease;
        }

        public void CallOffTheBoard(ref bool shouldDestroyShip, Direction direction)
        {
            if (OnOffTheBoard != null) OnOffTheBoard(ref shouldDestroyShip, direction);
        }

        public event EventHandlerShipRefInt OnModifyAIStressPriority;

        /// <summary>
        /// This number can be used by the AI to determine who to assign stress to when there is a choice, 
        /// or whether or not to use an ability or maneuver that causes stress.
        /// Abilities and upgrades that makes stress more or less harmful should modify this number.
        /// </summary>
        public int GetAIStressPriority()
        {
            /*
             * Default value for a ship is -50 + the number of blue maneuvers on its dial.
             * 
             * 100 - Always wants more stress tokens
             * 90 - Wants to have multiple stress tokens (Ten Numb)
             * 50 - Stress is a good thing, but not too much
             * 0 - Stress is neither negative or positive for this ship
             * -50 - Stress is always negative
             * -100 - Stress is really bad for this ship, avoid at all cost!
             */

            var blueManeuverCount = this.Maneuvers.Values.Count(m => m == Movement.MovementComplexity.Easy);
            var result = -50 + blueManeuverCount; 

            if (OnModifyAIStressPriority != null) OnModifyAIStressPriority(this, ref result);

            return result;
        }

        // Squadbuilder

        public void CallOnPreInstallUpgrade(GenericUpgrade upgrade)
        {
            if (OnPreInstallUpgrade != null) OnPreInstallUpgrade(upgrade);
        }

        public void CallOnRemovePreInstallUpgrade(GenericUpgrade upgrade)
        {
            if (OnRemovePreInstallUpgrade != null) OnRemovePreInstallUpgrade(upgrade);
        }

        // Setup filters

        public void CallOnSetupSelected()
        {
            if (OnSetupSelected != null) OnSetupSelected(this);
            if (OnSetupSelectedGlobal != null) OnSetupSelectedGlobal(this);
        }

        public void CallOnSetupPlaced()
        {
            if (OnSetupPlaced != null) OnSetupPlaced(this);
            if (OnSetupPlacedGlobal != null) OnSetupPlacedGlobal(this);
        }

        // Arcs and distance override

        public void CallOnBullseyeArcCheck(GenericShip anotherShip, ref bool result)
        {
            if (OnBullseyeArcCheck != null) OnBullseyeArcCheck(anotherShip, ref result);
        }

        public bool CallOnCheckRange(GenericShip anotherShip, int minRange, int maxRange, BoardTools.RangeCheckReason reason, bool isInRange)
        {
            if (OnCheckRange != null) OnCheckRange(anotherShip, minRange, maxRange, reason, ref isInRange);
            return isInRange;
        }

        // Teams

        public void CallTryAttackSameTeamCheck(GenericShip ship, ref bool result)
        {
            OnTryAttackSameTeamCheck?.Invoke(ship, ref result);
        }

        // Docking

        public Vector2 GetDockingRange(GenericShip carrier)
        {
            Vector2 dockingRange = new Vector2(0, 0);
            OnGetDockingRange?.Invoke(carrier, ref dockingRange);
            return dockingRange;
        }

        // Expose Damage Cards
        public void CallSelectDamageCardToExpose(int index, Action<int, Action, bool> exposeFacedownCardByIndex, Action callback)
        {
            bool isOverriden = false;
            OnSelectDamageCardToExpose?.Invoke(ref isOverriden);

            Triggers.ResolveTriggers(
                TriggerTypes.OnSelectDamageCardToExpose,
                delegate { exposeFacedownCardByIndex.Invoke(index, callback, isOverriden); }
            );
        }

        public void CallFaceupDamageCardIsRepaired(GenericDamageCard damageCard, Action callback)
        {
            OnFaceupDamageCardIsRepaired?.Invoke(damageCard);
            Triggers.ResolveTriggers(TriggerTypes.OnFaceupDamageCardIsRepaired, callback);
        }
    }

}
