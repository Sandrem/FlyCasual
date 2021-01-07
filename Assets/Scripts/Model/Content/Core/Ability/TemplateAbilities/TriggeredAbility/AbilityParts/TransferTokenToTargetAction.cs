using System;

namespace Abilities
{
    public class TransferTokenToTargetAction : AbilityPart
    {
        private GenericAbility Ability;
        public Type TokenType { get; }
        public Func<string> GetMessage { get; }

        public TransferTokenToTargetAction(Type tokenType, Func<string> showMessage)
        {
            TokenType = tokenType;
            GetMessage = showMessage;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            Messages.ShowInfo(GetMessage());
            Ability.HostShip.Tokens.TransferToken(TokenType, Ability.TargetShip, Triggers.FinishTrigger);
        }
    }
}
