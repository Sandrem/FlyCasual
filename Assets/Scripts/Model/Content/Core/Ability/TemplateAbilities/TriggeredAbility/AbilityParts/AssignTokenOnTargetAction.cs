using System;
using Tokens;

namespace Abilities
{
    public class AssignTokenOnTargetAction : AbilityPart
    {
        private TriggeredAbility Ability;
        public Type TokenType { get; }
        public Func<int> GetCount { get; }
        public Func<string> GetMessage { get; }

        public AssignTokenOnTargetAction(Type tokenType, Func<int> getCount, Func<string> showMessage)
        {
            TokenType = tokenType;
            GetCount = getCount;
            GetMessage = showMessage;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;
            Messages.ShowInfo(GetMessage());
            ability.TargetShip.Tokens.AssignTokens(CreateToken, GetCount(), Triggers.FinishTrigger);
        }

        private GenericToken CreateToken()
        {
            return (GenericToken) Activator.CreateInstance(TokenType, Ability.TargetShip);
        }
    }
}
