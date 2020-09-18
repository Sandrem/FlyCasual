using Ship;
using System;
using Tokens;

namespace Abilities
{
    public class AssignTokenAction : AbilityPart
    {
        private TriggeredAbility Ability;
        public Type TokenType { get; }
        public Func<GenericShip> GetShip { get; }
        public Func<int> GetCount { get; }
        public Func<string> GetMessage { get; }

        public AssignTokenAction(Type tokenType, Func<GenericShip> targetShip, Func<int> getCount = null, Func<string> showMessage = null)
        {
            TokenType = tokenType;
            GetShip = targetShip;
            GetCount = getCount;
            GetMessage = showMessage;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;
            Messages.ShowInfo(GetMessage());
            int count = (GetCount != null) ? GetCount() : 1;
            GetShip().Tokens.AssignTokens(CreateToken, count, Triggers.FinishTrigger);
        }

        private GenericToken CreateToken()
        {
            return (GenericToken) Activator.CreateInstance(TokenType, GetShip());
        }
    }
}
