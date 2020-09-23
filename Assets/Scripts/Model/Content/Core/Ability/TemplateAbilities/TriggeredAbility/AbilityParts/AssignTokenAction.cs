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
        public AbilityPart AfterAction { get; }

        public AssignTokenAction
        (
            Type tokenType,
            Func<GenericShip> targetShip,
            Func<int> getCount = null,
            Func<string> showMessage = null,
            AbilityPart afterAction = null
        )
        {
            TokenType = tokenType;
            GetShip = targetShip;
            GetCount = getCount;
            GetMessage = showMessage;
            AfterAction = afterAction;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;
            Messages.ShowInfo(GetMessage());
            int count = (GetCount != null) ? GetCount() : 1;
            GetShip().Tokens.AssignTokens(CreateToken, count, FinishAbilityPart);
        }

        private GenericToken CreateToken()
        {
            return (GenericToken) Activator.CreateInstance(TokenType, GetShip());
        }

        private void FinishAbilityPart()
        {
            if (AfterAction != null)
            {
                AfterAction.DoAction(Ability);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
