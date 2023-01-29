using ActionsList;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class JulJerjerrod : TIESeBomber
        {
            public JulJerjerrod() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Jul Jerjerrod",
                    "Security Commander",
                    Faction.FirstOrder,
                    4,
                    4,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JulJerjerrodPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/f646cd72-d2a9-446e-82b6-66028abfcea5/SWZ97_JulJerjerrodlegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JulJerjerrodPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbilityAfterBoost;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbilityAfterBoost;
        }

        private void CheckAbilityAfterBoost(GenericAction action)
        {
            if (action is BoostAction && HostShip.State.Charges > 0 && HostShip.Tokens.GetNonLockRedOrangeTokens().Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskUseAbility);
            }
        }
        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                UseAbility,
                descriptionLong: "Do you want to spend 1 Charge to remove 1 non-lock red or orange token?",
                imageHolder: HostShip
            );
        }
        private void UseAbility(object sender, EventArgs e)
        {
            JulJerjerrodAbilitySubphase subphase = Phases.StartTemporarySubPhaseNew<JulJerjerrodAbilitySubphase>(
                "Remove Token",
                DecisionSubPhase.ConfirmDecision
            );

            subphase.Name = HostShip.PilotInfo.PilotName;
            subphase.DescriptionShort = "Select a non-lock red or orange token to remove";
            subphase.ImageSource = HostShip;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            HostShip.SpendCharges(1);

            List<GenericToken> tokensToRemove = HostShip.Tokens.GetNonLockRedOrangeTokens();

            foreach (GenericToken token in tokensToRemove)
            {
                subphase.AddDecision(
                    token.Name,
                    delegate {
                        tokensToRemove.Add(token);
                        ActionsHolder.RemoveTokens(tokensToRemove, DecisionSubPhase.ConfirmDecision);
                    }
                );
            }


            if (subphase.GetDecisions().Count > 0)
            {
                subphase.Start();
            }
            else
            {
                Phases.GoBack();
                Messages.ShowInfoToHuman("Jul Jerjerrod: No tokens to remove");
                Triggers.FinishTrigger();
            }
        }

        private class JulJerjerrodAbilitySubphase : DecisionSubPhase { }
    }
}