﻿using Ship;
using System.Linq;

namespace ActionsList
{
    public abstract class FriendlyRerollAction : GenericAction
    {
        public enum RerollTypeEnum
        {
            AttackDice,
            DefenseDice
        }

        protected int NumberOfDice = 0;
        protected int MaxFriendlyShipRange = 0;
        protected bool CanUseOwnAbility;
        protected RerollTypeEnum RerollType;

        public FriendlyRerollAction(int numberOfDice, int maxFriendlyShipRange, bool canUseOwnAbility, RerollTypeEnum rerollType)
        {
            MaxFriendlyShipRange = maxFriendlyShipRange;
            NumberOfDice = numberOfDice;
            CanUseOwnAbility = canUseOwnAbility;
            RerollType = rerollType;

            Name = DiceModificationName = string.Format("{0}'s ability", Name);
            IsReroll = true;
        }

        protected virtual bool CanReRollWithWeaponClass()
        {
            return true;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == (RerollType == RerollTypeEnum.AttackDice ? CombatStep.Attack : CombatStep.Defence))
            {
                var friendlyShip = RerollType == RerollTypeEnum.AttackDice ? Combat.Attacker : Combat.Defender;

                if (CanReRollWithWeaponClass())
                {
                    if (CanUseOwnAbility || friendlyShip != HostShip)
                    {
                        if (friendlyShip.Owner == HostShip.Owner)
                        {
                            BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(HostShip, friendlyShip);
                            if (positionInfo.Range <= MaxFriendlyShipRange)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == (RerollType == RerollTypeEnum.AttackDice ? CombatStep.Attack : CombatStep.Defence))
            {
                var friendlyShip = RerollType == RerollTypeEnum.AttackDice ? Combat.Attacker : Combat.Defender;
                int focuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int blanks = Combat.DiceRollAttack.BlanksNotRerolled;

                //if (friendlyShip.HasToken(typeof(Tokens.FocusToken)))
                if (friendlyShip.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (blanks > 0) result = 90;
                }
                else
                {
                    if (blanks + focuses > 0) result = 90;
                }
            }
            
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = NumberOfDice,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }
    }
}
