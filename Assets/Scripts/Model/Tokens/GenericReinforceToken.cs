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
                if (RuleSets.RuleSet.Instance.ReinforceEffectCanBeUsed(Facing))
                {
                    Messages.ShowInfo("Reinforce: Evade result is added");
                    diceroll.AddDice(DieSide.Success).ShowWithoutRoll();
                    diceroll.OrganizeDicePositions();
                }
            }
        }

        protected virtual void ApplyPostCombatReinforceEffect(GenericShip ship)
        {
            if (RuleSets.RuleSet.Instance.ReinforcePostCombatEffectCanBeUsed(Facing))
            {
                DieSide dieSideToChange = DieSide.Unknown;
                if (Combat.DiceRollAttack.RegularSuccesses > 0)
                {
                    dieSideToChange = DieSide.Success;
                    Messages.ShowInfo("Reinforce: Hit is cancelled");
                }
                else if (Combat.DiceRollAttack.CriticalSuccesses > 0)
                {
                    dieSideToChange = DieSide.Crit;
                    Messages.ShowInfo("Reinforce: Crit is cancelled");
                }
                Combat.DiceRollAttack.ChangeOne(dieSideToChange, DieSide.Blank);
            }
        }
    }

}
