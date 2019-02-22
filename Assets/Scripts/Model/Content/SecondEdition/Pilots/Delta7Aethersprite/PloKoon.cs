using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class PloKoon : Delta7Aethersprite
    {
        public PloKoon()
        {
            PilotInfo = new PilotCardInfo(
                "Plo Koon",
                5,
                63,
                true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.PloKoonAbility),
                extraUpgradeIcon: UpgradeType.Force
            );

            ModelInfo.SkinName = "Plo Koon";

            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6a/6f/6a6fef51-fb5f-49c1-b5cc-8e96b6d09051/swz32_plo-koon.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagement Phase, you may spend 1 force and choose another friendly ship at range 0-2. 
    //If you do, you may transfer 1 green token to it or transfer one orange token from it to you.
    public class PloKoonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if (HostShip.State.Force > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
            }
        }

        private void Ability(object sender, EventArgs e)
        {
            if (TargetsForAbilityExist(FilterAbilityTarget))
            {
                Messages.ShowInfoToHuman(HostName + ": Select a ship to transfer a token to or from");

                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "Choose a ship to transfer 1 green token to it or transfer one orange token from it to you.",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SelectAbilityTarget()
        {
            var greenTokens = HostShip.Tokens.GetAllTokens()
                .Where(token => token.TokenColor == TokenColors.Green)
                .Distinct(new TokenComparer())
                .ToList();

            var orangeTokens = TargetShip.Tokens.GetAllTokens()
                .Where(token => token.TokenColor == TokenColors.Orange)
                .Distinct(new TokenComparer())
                .ToList();

            if (greenTokens.Any() || orangeTokens.Any()) { 

                DecisionSubPhase phase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(DecisionSubPhase),
                    Triggers.FinishTrigger
                );

                phase.InfoText = "Select token to transfer";
                phase.RequiredPlayer = HostShip.Owner.PlayerNo;
                phase.ShowSkipButton = true;

                greenTokens.ForEach(token =>
                {
                    phase.AddDecision(token.Name, delegate { TransferToken(token.GetType(), HostShip, TargetShip); });
                });

                orangeTokens.ForEach(token =>
                {
                    phase.AddDecision(token.Name, delegate { TransferToken(token.GetType(), TargetShip, HostShip); });
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
            HostShip.State.Force--;
            fromShip.Tokens.TransferToken(tokenType, toShip, () =>
            {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                SelectShipSubPhase.FinishSelection();
            });
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            //TODO: when should the AI use this ability?   
            return 0;
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 0, 2);
        }
    }
}
