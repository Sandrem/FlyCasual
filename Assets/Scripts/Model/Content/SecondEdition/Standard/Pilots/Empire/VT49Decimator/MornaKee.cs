using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VT49Decimator
    {
        public class MornaKee : VT49Decimator
        {
            public MornaKee() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Morna Kee",
                    "Determined Attaché",
                    Faction.Imperial,
                    4,
                    7,
                    22,
                    isLimited: true,
                    charges: 3,
                    abilityType: typeof(Abilities.SecondEdition.MornaKeeAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/53/16/53162173-c0c4-423e-8bbe-d5d0be9554cb/swz66_morna-kee.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MornaKeeAbility : GenericAbility
    {
        //During the end phase, you may spend 1 charge to flip 1 of your reinforce token to your other full arc instead of removing it.
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if (HostShip.Tokens.HasToken<ReinforceForeToken>() || HostShip.Tokens.HasToken<ReinforceAftToken>())
            {
                if (HostShip.State.Charges > 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskAbility);
                }
            }
        }

        private void AskAbility(object sender, EventArgs e)
        {
            TokenToKeep = null;

            if (!(HostShip.Tokens.HasToken<ReinforceForeToken>() || HostShip.Tokens.HasToken<ReinforceAftToken>()))
                return;

            if (HostShip.State.Charges == 0)
                return;

            var decisionSubPhase = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(
                    Name,
                    Triggers.FinishTrigger
                );

            decisionSubPhase.DescriptionShort = "Spend 1 charge to flip 1 reinforce token instead of removing it?";

            if (HostShip.Tokens.HasToken<ReinforceForeToken>())
                decisionSubPhase.AddDecision(
                    "Fore > Aft",
                    delegate {
                        FlipToken(true);
                    }
                );

            if (HostShip.Tokens.HasToken<ReinforceAftToken>())
                decisionSubPhase.AddDecision(
                     "Aft > Fore",
                     delegate {
                         FlipToken(false);
                     }
                 );

            decisionSubPhase.DefaultDecisionName = decisionSubPhase.GetDecisions().First().Name;
            decisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            decisionSubPhase.ShowSkipButton = true;

            decisionSubPhase.Start();
        }

        Type TokenToKeep;

        private void FlipToken(bool flipFore)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            
            Type tokenToRemove = flipFore ? typeof(ReinforceForeToken) : typeof(ReinforceAftToken);
            Type tokenToAssign = flipFore ? typeof(ReinforceAftToken) : typeof(ReinforceForeToken);

            if (HostShip.State.Charges > 0 && HostShip.Tokens.HasToken(tokenToRemove))
            { 
                Messages.ShowInfo(HostName + " flips 1 " + (flipFore ? "fore" : "aft") + " reinforce to " + (flipFore ? "aft" : "fore"));

                HostShip.BeforeRemovingTokenInEndPhase += KeepReinforce;
                HostShip.State.Charges--;
                TokenToKeep = tokenToAssign;
                HostShip.Tokens.RemoveToken(tokenToRemove, () =>
                {
                    HostShip.Tokens.AssignToken(tokenToAssign, Triggers.FinishTrigger);
                });
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void KeepReinforce(GenericShip ship, GenericToken token, ref bool remove)
        {
            if (token.GetType() == TokenToKeep)
            {
                HostShip.BeforeRemovingTokenInEndPhase -= KeepReinforce;
                remove = false;
                TokenToKeep = null;
            }
        }
    }
}
