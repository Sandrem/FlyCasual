using System;
using System.Collections.Generic;
using Ship;
using SubPhases;
using UnityEngine;
using Upgrade;
using Players;
using System.Linq;
using ActionsList;

namespace Abilities
{
    public abstract class GenericAbility
    {
        public string Name { get; private set; }

        /// <summary>
        /// Set to true if ability applies condition card. Is checked by Thweek.
        /// </summary>
        public bool IsAppliesConditionCard { get; protected set; }

        /// <summary>
        /// Use to check is ability used
        /// </summary>
        protected bool IsAbilityUsed { get; set; }

        private object hostReal;
        /// <summary>
        /// Real host of ability - object of ship or upgrade
        /// </summary>
        public object HostReal
        {
            get { return hostReal; }
            private set { hostReal = value; }
        }

        private GenericShip hostShip;
        /// <summary>
        /// Ship that is host of ability (pilot's ability or ability of installed upgrade)
        /// </summary>
        public GenericShip HostShip
        {
            get { return hostShip; }
            private set { hostShip = value; }
        }

        private GenericUpgrade hostUpgrade;
        /// <summary>
        /// Upgrade that is host of ability
        /// </summary>
        public GenericUpgrade HostUpgrade
        {
            get { return hostUpgrade; }
            private set { hostUpgrade = value; }
        }

        /// <summary>
        /// Name of host (ship or upgrade)
        /// </summary>
        public string HostName
        {
            get
            {
                return HostUpgrade != null ? HostUpgrade.UpgradeInfo.Name : HostShip.PilotInfo.PilotName;
            }
        }

        /// <summary>
        /// Image url of host (ship or upgrade)
        /// </summary>
        public string HostImageUrl
        {
            get
            {
                return HostUpgrade != null ? HostUpgrade.ImageUrl : HostShip.ImageUrl;
            }
        }

        public virtual void Initialize(GenericShip hostShip)
        {
            InitializeForSquadBuilder(hostShip);
            ActivateAbility();
        }

        public virtual void InitializeForSquadBuilder(GenericShip hostShip)
        {
            HostReal = hostShip;
            HostShip = hostShip;
            Name = HostShip.PilotInfo.PilotName + "'s ability";

            ActivateAbilityForSquadBuilder();
        }

        public virtual void InitializeForSquadBuilder(GenericUpgrade hostUpgrade)
        {
            HostReal = hostUpgrade;
            HostShip = hostUpgrade.HostShip;
            HostUpgrade = hostUpgrade;
            Name = hostUpgrade.UpgradeInfo.Name + "'s ability";

            ActivateAbilityForSquadBuilder();
        }

        // ACTIVATE AND DEACTIVATE

        /// <summary>
        /// Turn on ability of card
        /// </summary>
        public abstract void ActivateAbility();

        /// <summary>
        /// Turn on ability of upgrade during squad building
        /// </summary>
        public virtual void ActivateAbilityForSquadBuilder() { }

        /// <summary>
        /// Turn off ability of card
        /// </summary>
        public abstract void DeactivateAbility();

        /// <summary>
        /// Turn off ability of upgrade during squad building
        /// </summary>
        public virtual void DeactivateAbilityForSquadBuilder() { }

        // REGISTER TRIGGER

        /// <summary>
        /// Register trigger of ability
        /// </summary>
        protected Trigger RegisterAbilityTrigger(TriggerTypes triggerType, EventHandler eventHandler, System.EventArgs e = null)
        {
            var trigger = new Trigger()
            {
                Name = Name,
                TriggerType = triggerType,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = eventHandler,
                Sender = hostReal,
                EventArgs = e
            };
            Triggers.RegisterTrigger(trigger);
            return trigger;
        }

        // DECISION USE ABILITY YES/NO

        /// <summary>
        /// Stores "Always use" decision of player for this ability
        /// </summary>
        protected bool alwaysUseAbility;

        /// <summary>
        /// Shows "Take a decision" window for ability with Yes / No / [Always] buttons
        /// </summary>
        protected void AskToUseAbility(Func<bool> useByDefault, EventHandler useAbility, EventHandler dontUseAbility = null, Action callback = null, bool showAlwaysUseOption = false, string infoText = null, bool showSkipButton = true)
        {
            if (dontUseAbility == null) dontUseAbility = DontUseAbility;

            if (callback == null) callback = Triggers.FinishTrigger;

            DecisionSubPhase pilotAbilityDecision = (DecisionSubPhase) Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(AbilityDecisionSubphase),
                callback
            );

            pilotAbilityDecision.InfoText = infoText ?? "Use " + Name + "?";

            pilotAbilityDecision.RequiredPlayer = HostShip.Owner.PlayerNo;

            pilotAbilityDecision.AddDecision("Yes", useAbility);
            pilotAbilityDecision.AddDecision("No", dontUseAbility);
            if (showAlwaysUseOption) pilotAbilityDecision.AddDecision("Always", delegate { SetAlwaysUse(useAbility); });

            pilotAbilityDecision.DefaultDecisionName = (useByDefault()) ? "Yes" : "No";

            pilotAbilityDecision.ShowSkipButton = showSkipButton;

            pilotAbilityDecision.Start();
        }

        /// <summary>
        /// Shows "Take a decision" window for ability with Yes / No buttons to Opponent
        /// </summary>
        protected void AskOpponent(Func<bool> aiUseByDefault, EventHandler useAbility, EventHandler dontUseAbility = null, string infoText = null, bool showSkipButton = true)
        {
            if (dontUseAbility == null) dontUseAbility = DontUseAbility;

            DecisionSubPhase opponentDecision = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(AbilityDecisionSubphase),
                Triggers.FinishTrigger
            );

            opponentDecision.InfoText = infoText ?? "Allow to use " + Name + "?";

            opponentDecision.RequiredPlayer = Roster.AnotherPlayer(HostShip.Owner.PlayerNo);

            opponentDecision.AddDecision("Yes", useAbility);
            opponentDecision.AddDecision("No", dontUseAbility);

            opponentDecision.DefaultDecisionName = (aiUseByDefault()) ? "Yes" : "No";

            opponentDecision.ShowSkipButton = showSkipButton;

            opponentDecision.Start();
        }

        private class AbilityDecisionSubphase : DecisionSubPhase { }

        private void DontUseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        /// <summary>
        /// Use in AskToUseAbility to always use ability by AI
        /// </summary>
        protected bool AlwaysUseByDefault()
        {
            return true;
        }

        /// <summary>
        /// Use in AskToUseAbility to always prevent using of this ability by AI
        /// </summary>
        protected bool NeverUseByDefault()
        {
            return false;
        }

        protected void SetAlwaysUse(EventHandler useAbility)
        {
            alwaysUseAbility = true;
            useAbility(null, null);
        }

        // SELECT SHIP AS TARGET OF ABILITY

        protected GenericShip TargetShip;

        /// <summary>
        /// Starts "Select ship for ability" subphase
        /// </summary>
        protected void SelectTargetForAbility(Action selectTargetAction, Func<GenericShip, bool> filterTargets, Func<GenericShip, int> getAiPriority, PlayerNo subphaseOwnerPlayerNo, string name = null, string description = null, IImageHolder imageSource = null, bool showSkipButton = true)
        {
            Selection.ChangeActiveShip("ShipId:" + HostShip.ShipId);

            SelectShipSubPhase selectTargetSubPhase = (SelectShipSubPhase)Phases.StartTemporarySubPhaseNew(
                name,
                typeof(AbilitySelectTarget),
                Triggers.FinishTrigger
            );

            selectTargetSubPhase.PrepareByParameters(
                delegate { SelectShipForAbility(selectTargetAction); },
                filterTargets,
                getAiPriority,
                subphaseOwnerPlayerNo,
                showSkipButton,
                name,
                description,
                imageSource
            );

            selectTargetSubPhase.Start();
        }

        protected bool FilterByTargetType(GenericShip ship, List<TargetTypes> targetTypes)
        {
            return FilterByTargetType(ship, targetTypes.ToArray());
        }

        protected bool FilterByTargetType(GenericShip ship, params TargetTypes[] targetTypes)
        {
            bool result = false;

            if (targetTypes.Contains(TargetTypes.Enemy) && ship.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo) result = true;

            if (targetTypes.Contains(TargetTypes.This) && ship.ShipId == Selection.ThisShip.ShipId) result = true;

            if (targetTypes.Contains(TargetTypes.OtherFriendly) && ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo && ship.ShipId != Selection.ThisShip.ShipId) result = true;

            return result;
        }

        protected bool FilterTargetsByRange(GenericShip ship, int minRange, int maxRange)
        {
            bool result = true;

            if ((Phases.CurrentSubPhase as SelectShipSubPhase) == null || (Phases.CurrentSubPhase as SelectShipSubPhase).CanMeasureRangeBeforeSelection)
            {
                BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Selection.ThisShip, ship);
                if (distanceInfo.Range < minRange) return false;
                if (distanceInfo.Range > maxRange) return false;
            }

            return result;
        }

        /// <summary>
        /// Checks if ships that can be selected by abilities are present
        /// </summary>
        protected bool TargetsForAbilityExist(Func<GenericShip, bool> filter)
        {
            Selection.ChangeActiveShip("ShipId:" + HostShip.ShipId);

            return Roster.AllShips.Values.FirstOrDefault(n => filter(n)) != null;
        }

        private void SelectShipForAbility(Action selectTargetAction)
        {
            MovementTemplates.ReturnRangeRuler();

            TargetShip = (Phases.CurrentSubPhase as SelectShipSubPhase).TargetShip;
            selectTargetAction();
        }

        private class AbilitySelectTarget: SelectShipSubPhase
        {
            public override void RevertSubPhase() { }

            public override void SkipButton()
            {
                UI.HideSkipButton();
                FinishSelection();
            }
        }

        /// <summary>
        /// Use to clear isAbilityUsed flag
        /// </summary>
        protected void ClearIsAbilityUsedFlag()
        {
            IsAbilityUsed = false;
        }

        /// <summary>
        /// Use to set IsAbilityUsed to true
        /// </summary>
        protected void SetIsAbilityIsUsed(GenericShip ship)
        {
            IsAbilityUsed = true;
        }

        // DICE CHECKS

        protected DiceRoll DiceCheckRoll;

        /// <summary>
        /// Starts "Roll dice to check" subphase
        /// </summary>
        public void PerformDiceCheck(string description, DiceKind diceKind, int count, Action finish, Action callback)
        {
            Phases.CurrentSubPhase.Pause();

            Selection.ActiveShip = HostShip;

            AbilityDiceCheck subphase = Phases.StartTemporarySubPhaseNew<AbilityDiceCheck>(
                description,
                callback
            );

            subphase.DiceKind = diceKind;
            subphase.DiceCount = count;
            subphase.AfterRoll = delegate { AfterRollWrapper(finish); };

            subphase.Start();
        }

        private void AfterRollWrapper(Action callback)
        {
            AbilityDiceCheck subphase = Phases.CurrentSubPhase as AbilityDiceCheck;
            subphase.HideDiceResultMenu();
            DiceCheckRoll = subphase.CurrentDiceRoll;

            callback();
        }

        protected class AbilityDiceCheck : DiceRollCheckSubPhase
        {

            public static void ConfirmCheckNoCallback()
            {
                Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                Phases.CurrentSubPhase.Resume();
            }

            public static void ConfirmCheck()
            {
                Action callback = Phases.CurrentSubPhase.CallBack;
                ConfirmCheckNoCallback();
                callback();
            }

        };

        // DICE MODIFICATIONS

        protected enum DiceModificationType
        {
            Reroll,
            Change,
            Cancel
        }

        private GenericShip.EventHandlerShip DiceModification;

        /// <summary>
        /// Adds available dice modification
        /// </summary>
        protected void AddDiceModification(string name, Func<bool> isAvailable, Func<int> aiPriority, DiceModificationType modificationType, int count, List<DieSide> sidesCanBeSelected = null, DieSide sideCanBeChangedTo = DieSide.Unknown, DiceModificationTimingType timing = DiceModificationTimingType.Normal, bool isGlobal = false, Action<Action<bool>> payAbilityCost = null, Action payAbilityPostCost = null, bool isTrueReroll = true)
        {
            AddDiceModification(
                name,
                isAvailable,
                aiPriority,
                modificationType,
                delegate { return count; },
                sidesCanBeSelected,
                sideCanBeChangedTo,
                timing,
                isGlobal, 
                payAbilityCost,
                payAbilityPostCost,
                isTrueReroll
            );
        }

        /// <summary>
        /// Adds available dice modification
        /// </summary>
        protected void AddDiceModification(string name, Func<bool> isAvailable, Func<int> aiPriority, DiceModificationType modificationType, Func<int> getCount, List<DieSide> sidesCanBeSelected = null, DieSide sideCanBeChangedTo = DieSide.Unknown, DiceModificationTimingType timing = DiceModificationTimingType.Normal, bool isGlobal = false, Action<Action<bool>> payAbilityCost = null, Action payAbilityPostCost = null, bool isTrueReroll = true)
        {
            if (sidesCanBeSelected == null) sidesCanBeSelected = new List<DieSide>() { DieSide.Blank, DieSide.Focus, DieSide.Success, DieSide.Crit };

            DiceModification = (ship) =>
            {
                CustomDiceModification diceModification = new CustomDiceModification()
                {
                    Name = name,
                    DiceModificationName = name,
                    ImageUrl = HostImageUrl,
                    DiceModificationTiming = timing,
                    HostShip = HostShip,
                    Source = HostUpgrade,
                    CheckDiceModificationAvailable = isAvailable,
                    GenerateDiceModificationAiPriority = aiPriority,
                    DoDiceModification = (Action callback) =>
                    {
                        if (payAbilityCost == null) payAbilityCost = payCallback => payCallback(true);

                        payAbilityCost(success =>
                        {
                            if (success)
                            {
                                GenericDiceModification(
                                    delegate
                                    {
                                        if (payAbilityPostCost != null) payAbilityPostCost();
                                        callback();
                                    },
                                    modificationType,
                                    getCount,
                                    sidesCanBeSelected,
                                    sideCanBeChangedTo,
                                    timing,
                                    isTrueReroll
                                );
                            }
                            else callback();
                        });
                    },
                    IsReroll = modificationType == DiceModificationType.Reroll,
                };
                ship.AddAvailableDiceModification(diceModification);
            };

            if (!isGlobal)
            {
                switch (timing)
                {
                    case DiceModificationTimingType.AfterRolled:
                        HostShip.OnGenerateDiceModificationsAfterRolled += DiceModification;
                        break;
                    case DiceModificationTimingType.Normal:
                        HostShip.OnGenerateDiceModifications += DiceModification;
                        break;
                    case DiceModificationTimingType.Opposite:
                        HostShip.OnGenerateDiceModificationsOpposite += DiceModification;
                        break;
                    case DiceModificationTimingType.CompareResults:
                        HostShip.OnGenerateDiceModificationsCompareResults += DiceModification;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (timing)
                {
                    case DiceModificationTimingType.AfterRolled:
                        GenericShip.OnGenerateDiceModificationsAfterRolledGlobal += DiceModification;
                        break;
                    case DiceModificationTimingType.Normal:
                        GenericShip.OnGenerateDiceModificationsGlobal += DiceModification;
                        break;
                    case DiceModificationTimingType.Opposite:
                        GenericShip.OnGenerateDiceModificationsOppositeGlobal += DiceModification;
                        break;
                    case DiceModificationTimingType.CompareResults:
                        GenericShip.OnGenerateDiceModificationsCompareResultsGlobal += DiceModification;
                        break;
                    default:
                        break;
                }
            }
        }

        protected class CustomDiceModification : GenericAction { }

        private void GenericDiceModification(Action callback, DiceModificationType modificationType, Func<int> getCount, List<DieSide> sidesCanBeSelected, DieSide newSide, DiceModificationTimingType timing, bool isTrueReroll = true)
        {
            switch (modificationType)
            {
                case DiceModificationType.Reroll:
                    DiceModificationReroll(callback, getCount, sidesCanBeSelected, timing, isTrueReroll);
                    break;
                case DiceModificationType.Change:
                    DiceModificationChange(callback, getCount, sidesCanBeSelected, newSide);
                    break;
                case DiceModificationType.Cancel:
                    DiceModificationCancel(callback, sidesCanBeSelected, timing);
                    break;
                default:
                    break;
            }
        }

        private void DiceModificationChange(Action callback, Func<int> getCount, List<DieSide> sidesCanBeSelected, DieSide newSide)
        {
            //TODO: Change to select dice manager

            DieSide oldSide = DieSide.Unknown;
            foreach (DieSide side in sidesCanBeSelected)
            {
                if (DiceRoll.CurrentDiceRoll.HasResult(side))
                {
                    oldSide = side;
                    break;
                }
            }

            if (oldSide != DieSide.Unknown)
            {
                Combat.CurrentDiceRoll.Change(oldSide, newSide, getCount());
                callback();
            }
            else
            {
                callback();
            }
        }

        private void DiceModificationReroll(Action callback, Func<int> getCount, List<DieSide> sidesCanBeSelected, DiceModificationTimingType timing, bool isTrueReroll = true)
        {
            int diceCount = getCount();

            if (diceCount > 0)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = diceCount,
                    SidesCanBeRerolled = sidesCanBeSelected,
                    IsOpposite = timing == DiceModificationTimingType.Opposite,
                    IsTrueReroll = isTrueReroll,
                    CallBack = callback
                };
                diceRerollManager.Start();
            }
            else
            {
                Messages.ShowErrorToHuman("0 dice can be rerolled");
                callback();
            }
        }

        private void DiceModificationCancel(Action callback, List<DieSide> sidesCanBeSelected, DiceModificationTimingType timing)
        {
            List<Die> diceToCancel = DiceRoll.CurrentDiceRoll.DiceList.Where(d => sidesCanBeSelected.Contains(d.Side)).ToList();

            foreach (Die die in diceToCancel)
            {
                DiceRoll.CurrentDiceRoll.DiceList.Remove(die);
                die.RemoveModel();
            }

            //DiceRoll.CurrentDiceRoll.OrganizeDicePositions();
        }

        /// <summary>
        /// Removes available dice modification
        /// </summary>
        protected void RemoveDiceModification()
        {
            HostShip.OnGenerateDiceModifications -= DiceModification;
            GenericShip.OnGenerateDiceModificationsGlobal -= DiceModification;

            HostShip.OnGenerateDiceModificationsOpposite -= DiceModification;
            GenericShip.OnGenerateDiceModificationsOppositeGlobal -= DiceModification;

            HostShip.OnGenerateDiceModificationsCompareResults -= DiceModification;
            GenericShip.OnGenerateDiceModificationsCompareResultsGlobal -= DiceModification;
        }

        private class ShipDamageEventArgs : EventArgs
        {
            public GenericShip Ship;
            public int Damage;
            public bool IsCritical;
        }

        protected void DealDamageToShips(List<GenericShip> ships, int damage, bool isCritical, Action callback)
        {
            foreach (var ship in ships)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, DealDamageToShip, new ShipDamageEventArgs() { Ship = ship, Damage = damage, IsCritical = isCritical });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        private void DealDamageToShip(object sender, System.EventArgs e)
        {
            var args = (e as ShipDamageEventArgs);
            GenericShip ship = args.Ship;
            var damage = args.IsCritical ? 0 : args.Damage;
            var critDamage = args.IsCritical ? args.Damage : 0;

            Messages.ShowInfo(ship.PilotName + " is dealt Critical Hit by destruction of Autopilot Drone");

            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
            {
                DamageType = DamageTypes.CardAbility,
                Source = HostShip
            };

            ship.Damage.TryResolveDamage(damage, damageArgs, Triggers.FinishTrigger, critDamage: critDamage);
        }
    }
}
