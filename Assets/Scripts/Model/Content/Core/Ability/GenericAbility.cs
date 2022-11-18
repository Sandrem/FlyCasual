using System;
using System.Collections.Generic;
using Ship;
using SubPhases;
using UnityEngine;
using Upgrade;
using Players;
using System.Linq;
using ActionsList;
using BoardTools;
using Arcs;
using Tokens;

namespace Abilities
{
    public enum ShipTypes
    {
        This,
        Friendly,
        OtherFriendly,
        Enemy,
        Any
    }

    public abstract class GenericAbility
    {
        public virtual string Name { get; private set; }

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
            protected set { hostShip = value; }
        }

        /// <summary>
        /// Upgrade that is host of ability
        /// </summary>
        public GenericUpgrade HostUpgrade { get; set; }

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

        private Func<GenericShip, bool> FilterDockableShips;

        public GenericToken TargetToken {get; set;}

        public virtual void Initialize(GenericShip hostShip)
        {
            InitializeForSquadBuilder(hostShip);
            ActivateAbility();
        }

        public virtual void InitializeForSquadBuilder(GenericShip hostShip)
        {
            HostReal = hostShip;
            HostShip = hostShip;
            Name = Name ?? HostShip.PilotInfo.PilotName + "'s ability";

            ActivateAbilityForSquadBuilder();
        }

        public virtual void InitializeForSquadBuilder(GenericUpgrade hostUpgrade)
        {
            HostReal = hostUpgrade;
            HostShip = hostUpgrade.HostShip;
            HostUpgrade = hostUpgrade;
            Name = Name ?? hostUpgrade.UpgradeInfo.Name + "'s ability";

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
        public Trigger RegisterAbilityTrigger(TriggerTypes triggerType, EventHandler eventHandler, System.EventArgs e = null, bool isSkippable = false, string customTriggerName = null)
        {
            var trigger = new Trigger()
            {
                Name = customTriggerName ?? Name,
                TriggerType = triggerType,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = eventHandler,
                Sender = hostReal,
                EventArgs = e,
                Skippable = isSkippable
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
        public void AskToUseAbility(string descriptionShort, Func<bool> useByDefault, EventHandler useAbility, EventHandler dontUseAbility = null, Action callback = null, bool showAlwaysUseOption = false, string descriptionLong = null, IImageHolder imageHolder = null, bool showSkipButton = true, PlayerNo requiredPlayer = PlayerNo.PlayerNone)
        {
            if (dontUseAbility == null) dontUseAbility = DontUseAbility;

            if (callback == null) callback = Triggers.FinishTrigger;

            DecisionSubPhase pilotAbilityDecision = (DecisionSubPhase) Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(AbilityDecisionSubphase),
                callback
            );

            pilotAbilityDecision.DescriptionShort = descriptionShort ?? "Use " + Name + "?";
            pilotAbilityDecision.DescriptionLong = descriptionLong;
            pilotAbilityDecision.ImageSource = imageHolder;

            pilotAbilityDecision.RequiredPlayer = (requiredPlayer == PlayerNo.PlayerNone) ? HostShip.Owner.PlayerNo : requiredPlayer;

            pilotAbilityDecision.AddDecision("Yes", useAbility);
            pilotAbilityDecision.AddDecision("No", dontUseAbility);
            if (showAlwaysUseOption) pilotAbilityDecision.AddDecision("Always", delegate { SetAlwaysUse(useAbility); });

            pilotAbilityDecision.DefaultDecisionName = (useByDefault()) ? "Yes" : "No";

            pilotAbilityDecision.ShowSkipButton = showSkipButton;

            pilotAbilityDecision.Start();
        }

        /// <summary>
        /// Shows "Take a decision" window for ability with various buttons
        /// </summary>
        public void AskForDecision(
            string descriptionShort,
            string descriptionLong = null,
            IImageHolder imageHolder = null,
            Dictionary<string, EventHandler> decisions = null,
            Dictionary<string, string> tooltips = null,
            string defaultDecision = null,
            Action callback = null,
            bool showSkipButton = true,
            PlayerNo requiredPlayer = PlayerNo.PlayerNone
        )
        {
            if (callback == null) callback = Triggers.FinishTrigger;

            DecisionSubPhase pilotAbilityDecision = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(AbilityDecisionSubphase),
                callback
            );

            pilotAbilityDecision.DescriptionShort = descriptionShort;
            pilotAbilityDecision.DescriptionLong = descriptionLong;
            pilotAbilityDecision.ImageSource = imageHolder;

            pilotAbilityDecision.RequiredPlayer = (requiredPlayer == PlayerNo.PlayerNone) ? HostShip.Owner.PlayerNo : requiredPlayer;

            foreach (var decision in decisions)
            {
                pilotAbilityDecision.AddDecision(
                    decision.Key,
                    delegate
                    {
                        DecisionSubPhase.ConfirmDecisionNoCallback();
                        decision.Value.Invoke(null, null);
                    }
                );
            }

            foreach (var tooltip in tooltips)
            {
                pilotAbilityDecision.AddTooltip(tooltip.Key, tooltip.Value);
            }

            pilotAbilityDecision.DefaultDecisionName = defaultDecision;
            pilotAbilityDecision.ShowSkipButton = showSkipButton;

            pilotAbilityDecision.Start();
        }

        /// <summary>
        /// Shows "Take a decision" window for ability with Yes / No buttons to Opponent
        /// </summary>
        protected void AskOpponent(
            Func<bool> aiUseByDefault,
            EventHandler useAbility,
            EventHandler dontUseAbility = null,
            string descriptionShort = null,
            string descriptionLong = null,
            IImageHolder imageSource = null,
            bool showSkipButton = true
        )
        {
            if (dontUseAbility == null) dontUseAbility = DontUseAbility;

            DecisionSubPhase opponentDecision = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(AbilityDecisionSubphase),
                Triggers.FinishTrigger
            );

            opponentDecision.DescriptionShort = descriptionShort ?? "Allow to use " + Name + "?";
            opponentDecision.DescriptionLong = descriptionLong ?? null;
            opponentDecision.ImageSource = imageSource ?? null;

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
        public bool AlwaysUseByDefault()
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

        public GenericShip TargetShip;

        /// <summary>
        /// Starts "Select ship for ability" subphase
        /// </summary>
        public void SelectTargetForAbility(Action selectTargetAction, Func<GenericShip, bool> filterTargets, Func<GenericShip, int> getAiPriority, PlayerNo subphaseOwnerPlayerNo, string name = null, string description = null, IImageHolder imageSource = null, bool showSkipButton = true, Action callback = null, Action onSkip = null)
        {
            if (callback == null) callback = Triggers.FinishTrigger;

            Selection.ChangeActiveShip("ShipId:" + HostShip.ShipId);

            SelectShipSubPhase selectTargetSubPhase = (SelectShipSubPhase)Phases.StartTemporarySubPhaseNew(
                name,
                typeof(AbilitySelectTarget),
                callback
            );

            selectTargetSubPhase.PrepareByParameters(
                delegate { SelectShipForAbility(selectTargetAction); },
                filterTargets,
                getAiPriority,
                subphaseOwnerPlayerNo,
                showSkipButton,
                name,
                description,
                imageSource,
                onSkip
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

            if (targetTypes.Contains(TargetTypes.Enemy) && ship.Owner.PlayerNo != hostShip.Owner.PlayerNo) result = true;

            if (targetTypes.Contains(TargetTypes.This) && ship.ShipId == hostShip.ShipId) result = true;

            if (targetTypes.Contains(TargetTypes.OtherAny) && ship.ShipId != hostShip.ShipId) result = true;

            if (targetTypes.Contains(TargetTypes.OtherFriendly) && ship.Owner.PlayerNo == hostShip.Owner.PlayerNo && ship.ShipId != hostShip.ShipId) result = true;

            return result;
        }

        public bool FilterTargetsByRange(GenericShip ship, int minRange, int maxRange)
        {
            bool result = true;

            if ((Phases.CurrentSubPhase as SelectShipSubPhase) == null || (Phases.CurrentSubPhase as SelectShipSubPhase).CanMeasureRangeBeforeSelection)
            {
                DistanceInfo distanceInfo = new DistanceInfo(hostShip, ship);
                if (distanceInfo.Range < minRange) return false;
                if (distanceInfo.Range > maxRange) return false;
            }

            return result;
        }

        public bool FilterTargetsByRangeInArc(GenericShip ship, int minRange, int maxRange)
        {
            bool result = true;

            if ((Phases.CurrentSubPhase as SelectShipSubPhase) == null || (Phases.CurrentSubPhase as SelectShipSubPhase).CanMeasureRangeBeforeSelection)
            {
                ShotInfo shotInfo = new ShotInfo(hostShip, ship, hostShip.PrimaryWeapons);
                if (!shotInfo.InArc) return false;
                if (shotInfo.Range < minRange) return false;
                if (shotInfo.Range > maxRange) return false;
            }

            return result;
        }

        public bool FilterTargetsByParameters(GenericShip ship, int minRange, int maxRange, ArcType arcType, TargetTypes targetTypes, Type tokenType = null, List<Type> shipTypesOnly = null)
        {
            bool result = true;

            if ((Phases.CurrentSubPhase as SelectShipSubPhase) == null || (Phases.CurrentSubPhase as SelectShipSubPhase).CanMeasureRangeBeforeSelection)
            {
                if (!Tools.CheckShipsTeam(ship, hostShip, targetTypes)) return false;

                if (tokenType != null && !ship.Tokens.HasToken(tokenType, '*')) return false;

                ShotInfo shotInfo = new ShotInfo(hostShip, ship, hostShip.PrimaryWeapons);
                if (arcType != ArcType.None && !shotInfo.InArcByType(arcType)) return false;
                if (shotInfo.Range < minRange) return false;
                if (shotInfo.Range > maxRange) return false;

                if (shipTypesOnly != null)
                {
                    bool meetsShipTypeCondition = false;
                    foreach (Type shipType in shipTypesOnly)
                    {
                        if (ship.GetType().IsSubclassOf(shipType))
                        {
                            meetsShipTypeCondition = true;
                            break;
                        }
                    }
                    if (!meetsShipTypeCondition) return false;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if ships that can be selected by abilities are present
        /// </summary>

        public List<GenericShip> GetTargetsForAbility(Func<GenericShip, bool> filter)
        {
            return Roster.AllShips.Values.Where(n => filter(n)).ToList();
        }

        public bool TargetsForAbilityExist(Func<GenericShip, bool> filter)
        {
            return GetTargetsForAbility(filter).Count > 0;
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
                base.SkipButton();

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

        public enum DiceModificationType
        {
            Reroll,
            Change,
            Cancel,
            Add
        }

        private List<Action> DiceModificationRemovers = new List<Action>();

        /// <summary>
        /// Adds available dice modification
        /// </summary>
        protected void AddDiceModification(
            string name,
            Func<bool> isAvailable,
            Func<int> aiPriority,
            DiceModificationType modificationType,
            int count,
            List<DieSide> sidesCanBeSelected = null,
            DieSide sideCanBeChangedTo = DieSide.Unknown,
            DiceModificationTimingType timing = DiceModificationTimingType.Normal,
            bool isGlobal = false,
            Action<Action<bool>> payAbilityCost = null,
            Action payAbilityPostCost = null,
            bool isTrueReroll = true,
            bool isForcedFullReroll = false,
            bool isForcedModification = false,
            bool canBeUsedFewTimes = false
        )
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
                isTrueReroll,
                isForcedFullReroll,
                isForcedModification,
                canBeUsedFewTimes
            );
        }

        /// <summary>
        /// Adds available dice modification
        /// </summary>
        protected void AddDiceModification(
            string name,
            Func<bool> isAvailable,
            Func<int> aiPriority,
            DiceModificationType modificationType,
            Func<int> getCount,
            List<DieSide> sidesCanBeSelected = null,
            DieSide sideCanBeChangedTo = DieSide.Unknown,
            DiceModificationTimingType timing = DiceModificationTimingType.Normal,
            bool isGlobal = false,
            Action<Action<bool>> payAbilityCost = null,
            Action payAbilityPostCost = null,
            bool isTrueReroll = true,
            bool isForcedFullReroll = false,
            bool isForcedModification = false,
            bool canBeUsedFewTimes = false
        )
        {
            if (sidesCanBeSelected == null) sidesCanBeSelected = new List<DieSide>() { DieSide.Blank, DieSide.Focus, DieSide.Success, DieSide.Crit };

            GenericShip.EventHandlerShip DiceModification = (ship) =>
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
                    IsForced = isForcedModification,
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
                                    isTrueReroll,
                                    isForcedFullReroll,
                                    isForcedModification
                                );
                            }
                            else callback();
                        });
                    },
                    IsReroll = modificationType == DiceModificationType.Reroll,
                    CanBeUsedFewTimes = canBeUsedFewTimes
                };

                if (!isGlobal)
                {
                    ship.AddAvailableDiceModificationOwn(diceModification);
                }
                else
                {
                    ship.AddAvailableDiceModification(diceModification, HostShip);
                }
            };
            
            if (!isGlobal)
            {
                switch (timing)
                {
                    case DiceModificationTimingType.AfterRolled:
                        HostShip.OnGenerateDiceModificationsAfterRolled += DiceModification;
                        DiceModificationRemovers.Add(() => HostShip.OnGenerateDiceModificationsAfterRolled -= DiceModification);
                        break;
                    case DiceModificationTimingType.Normal:
                        HostShip.OnGenerateDiceModifications += DiceModification;
                        DiceModificationRemovers.Add(() => HostShip.OnGenerateDiceModifications -= DiceModification);
                        break;
                    case DiceModificationTimingType.Opposite:
                        HostShip.OnGenerateDiceModificationsOpposite += DiceModification;
                        DiceModificationRemovers.Add(() => HostShip.OnGenerateDiceModificationsOpposite -= DiceModification);
                        break;
                    case DiceModificationTimingType.CompareResults:
                        HostShip.OnGenerateDiceModificationsCompareResults += DiceModification;
                        DiceModificationRemovers.Add(() => HostShip.OnGenerateDiceModificationsCompareResults -= DiceModification);
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
                        DiceModificationRemovers.Add(() => GenericShip.OnGenerateDiceModificationsAfterRolledGlobal -= DiceModification);
                        break;
                    case DiceModificationTimingType.Normal:
                        GenericShip.OnGenerateDiceModificationsGlobal += DiceModification;
                        DiceModificationRemovers.Add(() => GenericShip.OnGenerateDiceModificationsGlobal -= DiceModification);
                        break;
                    case DiceModificationTimingType.Opposite:
                        GenericShip.OnGenerateDiceModificationsOppositeGlobal += DiceModification;
                        DiceModificationRemovers.Add(() => GenericShip.OnGenerateDiceModificationsOppositeGlobal -= DiceModification);
                        break;
                    case DiceModificationTimingType.CompareResults:
                        GenericShip.OnGenerateDiceModificationsCompareResultsGlobal += DiceModification;
                        DiceModificationRemovers.Add(() => GenericShip.OnGenerateDiceModificationsCompareResultsGlobal -= DiceModification);
                        break;
                    default:
                        break;
                }
            }
        }

        protected class CustomDiceModification : GenericAction { }

        private void GenericDiceModification(
            Action callback,
            DiceModificationType modificationType,
            Func<int> getCount,
            List<DieSide> sidesCanBeSelected, 
            DieSide newSide,
            DiceModificationTimingType timing,
            bool isTrueReroll = true,
            bool isForcedFullReroll = false,
            bool isForcedModification = false
        )
        {
            switch (modificationType)
            {
                case DiceModificationType.Reroll:
                    DiceModificationReroll(callback, getCount, sidesCanBeSelected, timing, isTrueReroll, isForcedFullReroll, isForcedModification);
                    break;
                case DiceModificationType.Change:
                    DiceModificationChange(callback, getCount, sidesCanBeSelected, newSide);
                    break;
                case DiceModificationType.Cancel:
                    DiceModificationCancel(callback, getCount, sidesCanBeSelected, timing);
                    break;
                case DiceModificationType.Add:
                    DiceModificationAdd(callback, getCount, newSide);
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

        private void DiceModificationReroll(Action callback, Func<int> getCount, List<DieSide> sidesCanBeSelected, DiceModificationTimingType timing, bool isTrueReroll = true, bool isForcedFullReroll = false, bool isForcedModification = false)
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
                    IsForcedFullReroll = isForcedFullReroll,
                    IsForcedModification = isForcedModification,
                    CallBack = callback
                };
                diceRerollManager.Start();
            }
            else
            {
                Messages.ShowErrorToHuman("No dice are eligible to be rerolled");
                callback();
            }
        }

        private void DiceModificationCancel(Action callback, Func<int> getCount, List<DieSide> sidesCanBeSelected, DiceModificationTimingType timing)
        {
            int diceCount = getCount();
            for (int i = 0; i < diceCount; i++)
            {
                Die die = DiceRoll.CurrentDiceRoll.DiceList.Where(d => sidesCanBeSelected.Contains(d.Side)).FirstOrDefault();
                if (die != null)
                {
                    DiceRoll.CurrentDiceRoll.DiceList.Remove(die);
                    die.RemoveModel();
                }
                else
                {
                    break;
                }
            }

            //DiceRoll.CurrentDiceRoll.OrganizeDicePositions();

            callback();
        }

        public void DiceModificationAdd(Action callBack, Func<int> getCount, DieSide side)
        {
            // TODO: Replace this quick hack to real roll of a dice
            if (side == DieSide.Unknown)
            {
                List<DieSide> AttackDieSides = new List<DieSide>()
                {
                    DieSide.Crit,
                    DieSide.Success,
                    DieSide.Success,
                    DieSide.Success,
                    DieSide.Focus,
                    DieSide.Focus,
                    DieSide.Blank,
                    DieSide.Blank
                };

                int index = UnityEngine.Random.Range(0, AttackDieSides.Count);
                side = AttackDieSides[index];
            }

            for (int i = 0; i < getCount(); i++) Combat.CurrentDiceRoll.AddDiceAndShow(side);

            callBack();
        }

        /// <summary>
        /// Removes available dice modifications
        /// </summary>
        protected void RemoveDiceModification()
        {
            DiceModificationRemovers.ForEach(remove => remove());
        }

        private class ShipDamageEventArgs : EventArgs
        {
            public GenericShip Ship;
            public int Damage;
            public bool IsCritical;
        }

        protected void DealDamageToShip(GenericShip ship, int damage, bool isCritical, Action callback)
        {
            DealDamageToShips(new List<GenericShip> { ship }, damage, isCritical, callback);
        }

        protected void DealDamageToShips(List<GenericShip> ships, int damage, bool isCritical, Action callback)
        {
            foreach (var ship in ships)
            {
                var trigger = RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, DealDamageToShip, new ShipDamageEventArgs() { Ship = ship, Damage = damage, IsCritical = isCritical });
                trigger.Name = "Damage to " + ship.PilotInfo.PilotName + " #" + ship.ShipId;
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        private void DealDamageToShip(object sender, EventArgs e)
        {
            var args = (e as ShipDamageEventArgs);
            GenericShip ship = args.Ship;
            var damage = args.IsCritical ? 0 : args.Damage;
            var critDamage = args.IsCritical ? args.Damage : 0;

            Messages.ShowInfo(ship.PilotInfo.PilotName + " has been dealt a " + (args.IsCritical ? "Critical " : "")  + "Hit by " + HostName);

            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
            {
                DamageType = DamageTypes.CardAbility,
                Source = HostShip
            };

            ship.Damage.TryResolveDamage(damage, damageArgs, Triggers.FinishTrigger, critDamage: critDamage);
        }

        // DOCKING

        protected void ActivateDocking(Func<GenericShip, bool> filterDockableShips, Func<Direction, bool> filterUndockDirection = null)
        {
            FilterDockableShips = filterDockableShips;
            HostShip.FilterUndockDirection = filterUndockDirection ?? HostShip.FilterUndockDirection;

            Phases.Events.OnSetupStart += CheckInitialDockingAbility;
            HostShip.OnCheckSystemsAbilityActivation += CheckPotentialDockingShips;
            HostShip.OnSystemsAbilityActivation += RegisterDockingShips;
        }

        protected void DeactivateDocking()
        {
            Phases.Events.OnSetupStart -= CheckInitialDockingAbility;
            HostShip.OnCheckSystemsAbilityActivation -= CheckPotentialDockingShips;
            HostShip.OnSystemsAbilityActivation -= RegisterDockingShips;
        }

        private void CheckInitialDockingAbility()
        {
            if (GetDockableShips().Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSetupStart, AskInitialDocking);
            }
        }

        private List<GenericShip> GetDockableShips()
        {
            return HostShip.Owner.Ships
                .Where(s => FilterDockableShips(s.Value))
                .Select(n => n.Value)
                .ToList();
        }

        private void AskInitialDocking(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                "Docking",
                AlwaysUseByDefault,
                StartInitialDocking,
                descriptionLong: "Do you want to dock a ship?"
            );
        }

        private void StartInitialDocking(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            List<GenericShip> dockableShips = GetDockableShips();
            if (dockableShips.Count == 1)
            {
                Rules.Docking.Dock(HostShip, dockableShips.First());
                Triggers.FinishTrigger();
            }
            else
            {
                // Ask what ships to dock
                Triggers.FinishTrigger();
            }
        }

        private void CheckPotentialDockingShips(GenericShip thisShip, ref bool flag)
        {
            foreach (GenericShip ship in hostShip.Owner.Ships.Values)
            {
                if (FilterDockableShips(ship))
                {
                    DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                    Vector2 dockingRange = ship.GetDockingRange(HostShip);
                    if (dockingRange.x <= distInfo.Range && distInfo.Range <= dockingRange.y)
                    {
                        flag = true;
                    }
                }
            }
        }

        private void RegisterDockingShips(GenericShip thisShip)
        {
            foreach (GenericShip ship in hostShip.Owner.Ships.Values)
            {
                if (FilterDockableShips(ship))
                {
                    DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                    Vector2 dockingRange = ship.GetDockingRange(HostShip);
                    if (dockingRange.x <= distInfo.Range && distInfo.Range <= dockingRange.y)
                    {
                        PrepareAskToDock(ship);
                    }
                }
            }
        }

        private void PrepareAskToDock(GenericShip ship)
        {
            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Ask to Dock",
                    TriggerOwner = ship.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnSystemsAbilityActivation,
                    EventHandler = AskToDock,
                    Sender = ship
                }
            );
        }

        private void AskToDock(object sender, EventArgs e)
        {
            GenericShip dockingShip = sender as GenericShip;

            AskToUseAbility(
                "Docking",
                NeverUseByDefault,
                delegate { ConfirmDocking(dockingShip, HostShip); },
                descriptionLong: "Do you want to dock to " + HostShip.PilotInfo.PilotName + "?"
            );
        }

        private void ConfirmDocking(GenericShip dockingShip, GenericShip chosenHostShip)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Rules.Docking.Dock(chosenHostShip, dockingShip);
            Triggers.FinishTrigger();
        }

        public GenericShip GetShip(ShipRole shipRole)
        {
            switch (shipRole)
            {
                case ShipRole.HostShip : return HostShip;
                case ShipRole.ThisShip : return Selection.ThisShip;
                case ShipRole.Attacker : return Combat.Attacker;
                case ShipRole.Defender : return Combat.Defender;
                case ShipRole.TargetShip : return TargetShip;
                case ShipRole.CoordinatedShip : return HostShip.State.LastCoordinatedShip;
                default: return null;
            }
        }
    }
}
