using System;
using System.Collections.Generic;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class ZiziTlo : RZ2AWing
        {
            public ZiziTlo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Zizi Tlo",
                    5,
                    40,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ZiziTloAbility),
                    charges: 1,
                    regensCharges: true,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent } 
                );

                ModelInfo.SkinName = "Red";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/bf/4f/bf4f8372-b151-45ee-b38b-5c67bba7d2d8/swz66_zizi-tlo.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you defend or perform an attack, you may spend 1 charge to gain 1 focus or evade token.
    public class ZiziTloAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, ChooseToken);
            }
        }

        private void ChooseToken(object sender, EventArgs e)
        {
            if (HostShip.State.Charges > 0)
            {
                var decisionSubPhase = Phases.StartTemporarySubPhaseNew<DecisionSubPhase>(
                    Name,
                    Triggers.FinishTrigger
                );

                decisionSubPhase.DescriptionShort = HostName + ": Spend 1 charge to gain 1 focus or evade token?";

                decisionSubPhase.AddDecision(
                    "Focus",
                    delegate {
                        GainToken("Focus");
                    }
                );
                decisionSubPhase.AddDecision(
                     "Evade",
                     delegate {
                         GainToken("Evade");
                     }
                 );

                decisionSubPhase.DefaultDecisionName = HostShip.Tokens.HasToken<FocusToken>() ? "Evade" : "Focus";
                decisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
                decisionSubPhase.ShowSkipButton = true;

                decisionSubPhase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void GainToken(string tokenName)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            if (HostShip.State.Charges > 0)
            {
                var tokenType = tokenName == "Focus" ? typeof(FocusToken) : typeof(EvadeToken);

                Messages.ShowInfo(HostName + " gains 1 " + tokenName + " token");
                HostShip.State.Charges--;
                HostShip.Tokens.AssignToken(tokenType, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}