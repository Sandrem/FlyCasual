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
    namespace SecondEdition.GauntletFighter
    {
        public class BoKatanKryzeSeparatists : GauntletFighter
        {
            public BoKatanKryzeSeparatists() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Bo-Katan Kryze",
                    "Vizsla's Lieutenant",
                    Faction.Separatists,
                    4,
                    7,
                    20,
                    isLimited: true,
                    charges: 1,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.BoKatanKryzeSeparatistsAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Configuration,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "CIS"
                );

                PilotNameCanonical = "bokatankryze-separatistalliance";

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a4/Bokatankryze-separatist-alliance.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BoKatanKryzeSeparatistsAbility : GenericAbility
    {
        private GenericShip friendlyShip;
        public override void ActivateAbility()
        {
            GenericShip.OnMovementActivationStartGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementActivationStartGlobal += CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            friendlyShip = ship;

            if (HostShip.State.Charges > 0 && HasReasonToUseAbility())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskToUseBoKatanAbility);
            }
        }

        private bool HasReasonToUseAbility()
        {
            DistanceInfo distanceInfo = new DistanceInfo(HostShip, friendlyShip);
            return friendlyShip.Tokens.GetNonStressRedOrangeTokens().Count > 0 &&
                distanceInfo.Range <= 2;
        }

        private void AskToUseBoKatanAbility(object sender, EventArgs e)
        {

            AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    NeverUseByDefault,
                    UseAbility,
                    descriptionLong: "Do you want to receive 1 Strain Token to remove 1 non-stress red or orange token?",
                    imageHolder: HostShip
                );
        }

        private void UseAbility(object sender, EventArgs e)
        {
            BoKatanAbilitySubphase subphase = Phases.StartTemporarySubPhaseNew<BoKatanAbilitySubphase>(
                "Remove Token",
                DecisionSubPhase.ConfirmDecision
            );

            subphase.Name = HostShip.PilotInfo.PilotName;
            subphase.DescriptionShort = "Select a non-stress red or orange token to remove";
            subphase.ImageSource = HostShip;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            HostShip.SpendCharges(1);

            List<GenericToken> tokensToRemove = friendlyShip.Tokens.GetNonStressRedOrangeTokens();

            foreach (GenericToken token in tokensToRemove)
            {
                subphase.AddDecision(
                    token.Name + ((token.GetType() == typeof(RedTargetLockToken)) ? " \"" + (token as RedTargetLockToken).Letter + "\"" : ""),
                    delegate {
                        tokensToRemove.Add(token);
                        ActionsHolder.RemoveTokens(tokensToRemove, delegate { friendlyShip.Tokens.AssignToken(typeof(StrainToken), DecisionSubPhase.ConfirmDecision); });
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
                Messages.ShowInfoToHuman("Bo Katan: No tokens to remove from the target");
                Triggers.FinishTrigger();
            }
        }

        private class BoKatanAbilitySubphase : DecisionSubPhase { }
    }
}