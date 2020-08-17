using System;
using Tokens;

namespace Abilities
{
    public class AssignTokenAction : AbilityPart
    {
        private TriggeredAbility Ability;
        public Type TokenType { get; }
        public Func<int> GetCount { get; }

        public AssignTokenAction(Type tokenType)
        {
            TokenType = tokenType;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;
            // TODO: Add notification
            ability.HostShip.Tokens.AssignTokens(CreateToken, 1, Triggers.FinishTrigger);
        }

        private GenericToken CreateToken()
        {
            return (GenericToken) Activator.CreateInstance(TokenType, Ability.TargetShip);
        }
    }
}
