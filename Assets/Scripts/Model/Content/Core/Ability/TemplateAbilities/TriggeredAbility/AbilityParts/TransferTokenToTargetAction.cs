using System;

namespace Abilities
{
    public class TransferTokenToTargetAction : AbilityPart
    {
        private TriggeredAbility Ability;
        public Type TokenType { get; }
        public Func<string> GetMessage { get; }

        public TransferTokenToTargetAction(Type tokenType, Func<string> showMessage)
        {
            TokenType = tokenType;
            GetMessage = showMessage;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            Messages.ShowInfo(GetMessage());
            Ability.HostShip.Tokens.TransferToken(TokenType, Ability.TargetShip, Triggers.FinishTrigger);
        }
    }
}
