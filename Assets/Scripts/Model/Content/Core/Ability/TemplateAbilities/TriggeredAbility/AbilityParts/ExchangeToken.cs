using System;
using System.Linq;
using Ship;
using SubPhases;
using Tokens;

namespace Abilities
{
    public class ExchangeToken : AbilityPart
    {
        private GenericAbility Ability;

        public TokenColors GetByColor { get; }
        public TokenColors GiveByColor { get; }
        public Func<string> GetMessage { get; }
        public AbilityPart DoNext { get; }

        public ExchangeToken(TokenColors getByColor, TokenColors giveByColor, Func<string> showMessage, AbilityPart doNext)
        {
            GetByColor = getByColor;
            GiveByColor = giveByColor;
            GetMessage = showMessage;
            DoNext = doNext;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            StartSubPhase();
        }

        private void StartSubPhase()
        {
            var greenTokens = Ability.HostShip.Tokens.GetAllTokens()
                .Where(token => token.TokenColor == GiveByColor)
                .Distinct(new TokenComparer())
                .ToList();

            var orangeTokens = Ability.TargetShip.Tokens.GetAllTokens()
                .Where(token => token.TokenColor == GetByColor)
                .Distinct(new TokenComparer())
                .ToList();

            if (greenTokens.Any() || orangeTokens.Any())
            {
                DecisionSubPhase phase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    "Exchange token",
                    typeof(DecisionSubPhase),
                    SelectShipSubPhase.FinishSelection
                );

                phase.DescriptionShort = "Select token to transfer";
                phase.RequiredPlayer = Ability.HostShip.Owner.PlayerNo;
                phase.ShowSkipButton = true;

                greenTokens.ForEach(token =>
                {
                    phase.AddDecision("Give " + token.Name, delegate { TransferToken(token.GetType(), Ability.HostShip, Ability.TargetShip); });
                });

                orangeTokens.ForEach(token =>
                {
                    phase.AddDecision("Get " + token.Name, delegate { TransferToken(token.GetType(), Ability.TargetShip, Ability.HostShip); });
                });

                phase.Start();
            }
            else
            {
                SelectShipSubPhase.FinishSelection();
            }
        }

        private void TransferToken(Type tokenType, GenericShip fromShip, GenericShip toShip)
        {
            fromShip.Tokens.TransferToken(
                tokenType,
                toShip,
                () => {
                    DecisionSubPhase.ConfirmDecisionNoCallback();
                    //Triggers.FinishTrigger();
                },
                Ability.HostShip.Owner
            );

            DoNext.DoAction(Ability);
        }
    }
}
