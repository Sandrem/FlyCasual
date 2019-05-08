using Arcs;
using Ship;

namespace Tokens
{

    public class GenericReinforceToken : GenericToken
    {
        public ArcFacing Facing;

        public GenericReinforceToken(GenericShip host): base(host)
        {
            TokenColor = TokenColors.Green;
            PriorityUI = 80;
        }

        public override void WhenAssigned()
        {
            Host.OnImmediatelyAfterRolling += ApplyReinforceEffect;
            Host.OnCombatCompareResults += ApplyPostCombatReinforceEffect;
        }

        public override void WhenRemoved()
        {
            Host.OnImmediatelyAfterRolling -= ApplyReinforceEffect;
            Host.OnCombatCompareResults -= ApplyPostCombatReinforceEffect;
        }

        protected virtual void ApplyReinforceEffect(DiceRoll diceroll)
        {
            if (diceroll.Type == DiceKind.Defence && diceroll.CheckType == DiceRollCheckType.Combat)
            {
                if (Editions.Edition.Current.ReinforceEffectCanBeUsed(Facing))
                {
                    Messages.ShowInfo("Reinforce has added an Evade result");
                    diceroll.AddDice(DieSide.Success).ShowWithoutRoll();
                    diceroll.OrganizeDicePositions();
                }
            }
        }

        protected virtual void ApplyPostCombatReinforceEffect(GenericShip ship)
        {
            if (Editions.Edition.Current.ReinforcePostCombatEffectCanBeUsed(Facing))
            {
                DieSide dieSideToChange = DieSide.Unknown;
                if (Combat.DiceRollAttack.RegularSuccesses > 0)
                {
                    dieSideToChange = DieSide.Success;
                    Messages.ShowInfo("Reinforce has cancelled a Hit");
                }
                else if (Combat.DiceRollAttack.CriticalSuccesses > 0)
                {
                    dieSideToChange = DieSide.Crit;
                    Messages.ShowInfo("Reinforce has cancelled a Critical Hit");
                }
                Combat.DiceRollAttack.ChangeOne(dieSideToChange, DieSide.Blank);
            }
        }
    }

}
