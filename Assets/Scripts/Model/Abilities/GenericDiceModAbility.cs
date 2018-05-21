using ActionsList;
using System;
using System.Collections.Generic;

namespace Abilities
{
    /// <summary>
    /// Abstract base class for abilities that modify combat dice
    /// </summary>
    public abstract class GenericDiceModAbility : GenericAbility
    {
        /// <summary>
        /// Used for abilities like Dark Curse's that can prevent rerolls
        /// </summary>
        protected bool? IsReroll = null;

        public DiceModificationTimingType DiceModificationTiming = DiceModificationTimingType.Normal;

        public abstract bool IsActionEffectAvailable();
        
        /// <summary>
        /// AI priority of effect. See GenericAction.cs for guidelines on what value to use
        /// </summary>
        public virtual int GetActionEffectPriority()
        {
            return 0;
        }

        private List<Action<Action>> ActionEffects = new List<Action<Action>>();

        /// <summary>
        /// Creates an action effect that allows rerolling dice
        /// </summary>
        /// <param name="numberOfDice">Number of dice that can be rerolled</param>
        /// <param name="sidesCanBeRerolled">List of die sides that can be rerolled. Default is all sides.</param>
        /// <param name="isOpposite">Set to true to reroll the opposing players dice</param>
        public void AllowReroll(int numberOfDice, List<DieSide> sidesCanBeRerolled = null, bool isOpposite = false)
        {
            if (IsReroll == null) IsReroll = true; 
            ActionEffects.Add(callback =>
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = numberOfDice,
                    SidesCanBeRerolled = sidesCanBeRerolled,
                    IsOpposite = isOpposite,
                    CallBack = callback
                };
                diceRerollManager.Start();
            });
        }

        /// <summary>
        /// Creates an action effect that allows changing a number dice of the same type. You can call this method multiple times to change different types of dice
        /// </summary>
        /// <param name="oldSide">Die side to change from</param>
        /// <param name="newSide">Die side to change to</param>
        /// <param name="count">Number of dice that will be changed, or all if not specified</param>
        /// <param name="cannotBeRerolled">Set to true to if the changed dice cannot be rerolled afterwards</param>
        /// <param name="cannotBeModified">Set to true if the changed dice cannot be further modified afterwards</param>
        public void AllowChange(DieSide oldSide, DieSide newSide, int? count = null, bool cannotBeRerolled = false, bool cannotBeModified = false)
        {
            ActionEffects.Add(callback =>
            {
                if (count == null) Combat.CurrentDiceRoll.ChangeAll(oldSide, newSide, cannotBeRerolled, cannotBeModified);
                else Combat.CurrentDiceRoll.Change(oldSide, newSide, count.Value, cannotBeRerolled, cannotBeModified);
                callback();
            });
        }

        /// <summary>
        /// Override this if you need more complex behaviour than AllowReroll() or AllowChange() provides
        /// </summary>
        public virtual void ActionEffect(Action callback)
        {
            if (ActionEffects.Count == 0) throw new InvalidOperationException("Must have at least one action effect");

            //chain the action effects together with callbacks
            Action<Queue<Action<Action>>> chainActionEffects = null;
            chainActionEffects = new Action<Queue<Action<Action>>>(queue =>
            {
                if (queue.Count > 0)
                {
                    var action = queue.Dequeue();
                    action(() => chainActionEffects(queue));
                }
                else
                {
                    callback();
                }
            });

            chainActionEffects(new Queue<Action<Action>>(ActionEffects));
        }

        public override void ActivateAbility()
        {
            switch (DiceModificationTiming)
            {
                case DiceModificationTimingType.Normal:
                case DiceModificationTimingType.Opposite:
                    HostShip.AfterGenerateAvailableActionEffectsList += AddActionEffect;
                    break;
                case DiceModificationTimingType.CompareResults:
                    HostShip.AfterGenerateAvailableCompareResultsEffectsList += AddActionEffect;
                    break;
            }            
        }

        public override void DeactivateAbility()
        {
            switch (DiceModificationTiming)
            {
                case DiceModificationTimingType.Normal:
                case DiceModificationTimingType.Opposite:
                    HostShip.AfterGenerateAvailableActionEffectsList -= AddActionEffect;
                    break;
                case DiceModificationTimingType.CompareResults:
                    HostShip.AfterGenerateAvailableCompareResultsEffectsList -= AddActionEffect;
                    break;
            }
        }

        private void AddActionEffect(Ship.GenericShip host)
        {
            var newAction = new GenericDiceModActionEffect(this);
            host.AddAvailableActionEffect(newAction);
        }

        protected class GenericDiceModActionEffect : GenericAction
        {
            protected GenericDiceModAbility SourceAbility;

            public GenericDiceModActionEffect(GenericDiceModAbility ability)
            {
                SourceAbility = ability;
                Host = ability.HostShip;
                Name = EffectName = ability.HostName;
                ImageUrl = ability.HostImageUrl;
                DiceModificationTiming = ability.DiceModificationTiming;
                IsReroll = ability.IsReroll == true;
            }

            public override bool IsActionEffectAvailable()
            {
                return SourceAbility.IsActionEffectAvailable();
            }

            public override int GetActionEffectPriority()
            {
                return SourceAbility.GetActionEffectPriority();
            }

            public override void ActionEffect(Action callback)
            {
                SourceAbility.ActionEffect(callback);
            }
        }
    }
}
