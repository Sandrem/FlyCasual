using Ship;
using System.Linq;

namespace ActionsList
{
    public abstract class FriendlyAttackRerollAction : ActionsList.GenericAction
    {
        protected int NumberOfDice = 0;
        protected int FriendlyShipRange = 0;

        public FriendlyAttackRerollAction(int numberOfDice, int friendlyShipRange)
        {
            FriendlyShipRange = friendlyShipRange;
            NumberOfDice = numberOfDice;

            Name = EffectName = string.Format("{0}'s ability", Name);
            IsReroll = true;
        }

        protected virtual bool CanReRollWithWeaponClass()
        {
            return Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass);
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (CanReRollWithWeaponClass())
                {
                    if (Combat.Attacker.ShipId != Host.ShipId)
                    {
                        if (Combat.Attacker.Owner.PlayerNo == Host.Owner.PlayerNo)
                        {
                            Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(Host, Combat.Attacker);
                            if (positionInfo.Range == FriendlyShipRange)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int attackBlanks = Combat.DiceRollAttack.BlanksNotRerolled;

                //if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken)))
                if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (attackBlanks > 0) result = 90;
                }
                else
                {
                    if (attackBlanks + attackFocuses > 0) result = 90;
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
