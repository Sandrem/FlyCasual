using System;
using Ship;

namespace Abilities
{
    public class TransferToken : AbilityPart
    {
        private GenericAbility Ability;
        public ShipRole TargetRole { get; }
        public Func<string> GetMessage { get; }

        public TransferToken(ShipRole target, Func<string> showMessage)
        {
            TargetRole = target;
            GetMessage = showMessage;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            GenericShip targetShip = Ability.GetShip(TargetRole);

            if (targetShip == null)
            {
                Messages.ShowError("Ability: Target of token transfer is missing");
                Triggers.FinishTrigger();
            }
            else if (Ability.TargetToken == null)
            {
                Messages.ShowInfoToHuman("Token is not transfered");
                Triggers.FinishTrigger();
            }
            else
            {
                Messages.ShowInfo(GetMessage());

                Ability.TargetToken.Host.Tokens.TransferToken
                (
                    Ability.TargetToken,
                    targetShip,
                    Triggers.FinishTrigger
                );
            }
        }
    }
}
