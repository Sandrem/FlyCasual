using Ship;
using System;
using Tokens;

namespace Abilities
{
    public class AssignTokenAction : AbilityPart
    {
        private GenericAbility Ability;
        public Type TokenType { get; }
        public ShipRole TargetShipRole { get; }
        public Func<int> GetCount { get; }
        public Func<string> GetMessage { get; }
        public AbilityPart AfterAction { get; }

        public AssignTokenAction
        (
            Type tokenType,
            ShipRole targetShipRole,
            Func<int> getCount = null,
            Func<string> showMessage = null,
            AbilityPart afterAction = null
        )
        {
            TokenType = tokenType;
            TargetShipRole = targetShipRole;
            GetCount = getCount;
            GetMessage = showMessage;
            AfterAction = afterAction;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;
            if (GetMessage != null) Messages.ShowInfo(GetMessage());
            int count = (GetCount != null) ? GetCount() : 1;
            Ability.GetShip(TargetShipRole).Tokens.AssignTokens(CreateToken, count, FinishAbilityPart);
        }

        private GenericToken CreateToken()
        {
            if (TokenType == typeof(JamToken) || TokenType == typeof(TractorBeamToken))
            {
                return (GenericToken) Activator.CreateInstance
                (
                    TokenType,
                    Ability.GetShip(TargetShipRole),
                    Ability.GetShip(TargetShipRole).Owner
                );
            }
            else
            {
                return (GenericToken) Activator.CreateInstance
                (
                    TokenType,
                    Ability.GetShip(TargetShipRole)
                );
            }
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
