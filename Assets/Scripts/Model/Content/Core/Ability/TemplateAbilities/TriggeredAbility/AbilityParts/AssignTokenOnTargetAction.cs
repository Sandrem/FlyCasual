using System;
using Tokens;

namespace Abilities
{
    public class AssignTokenOnTargetAction : AbilityPart
    {
        private TriggeredAbility Ability;
        public Type TokenType { get; }
        public Func<int> GetCount { get; }

        public AssignTokenOnTargetAction(Type tokenType, Func<int> getCount)
        {
            TokenType = tokenType;
            GetCount = getCount;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;
            // TODO: Add notification
            ability.TargetShip.Tokens.AssignTokens(CreateToken, GetCount(), Triggers.FinishTrigger);
        }

        private GenericToken CreateToken()
        {
            return (GenericToken) Activator.CreateInstance(TokenType, Ability.TargetShip);
        }
    }
}
