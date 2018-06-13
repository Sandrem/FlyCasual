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
        }

        public override void WhenAssigned()
        {
            Host.OnImmediatelyAfterRolling += ApplyReinforceEffect;
        }

        public override void WhenRemoved()
        {
            Host.OnImmediatelyAfterRolling -= ApplyReinforceEffect;
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
    }

}
