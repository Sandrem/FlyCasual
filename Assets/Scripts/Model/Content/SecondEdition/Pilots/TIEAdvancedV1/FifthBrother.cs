using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedV1
    {
        public class FifthBrother : TIEAdvancedV1
        {
            public FifthBrother() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Fifth Brother",
                    4,
                    42,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.FifthBrotherPilotAbility),
                    force: 2,
                    extraUpgradeIcon: UpgradeType.ForcePower
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/01a02a00ef5aad21bc1f0a58028136ec.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you perform an attack, after the Neutralize Results step, if the attack hit,
    // you may spend 2 force to add 1 crit result.

    public class FifthBrotherPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterFifthBrotherPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterFifthBrotherPilotAbility;
        }

        protected virtual void RegisterFifthBrotherPilotAbility()
        {
            if (HostShip.State.Force >= 2)
            {
                Phases.CurrentSubPhase.Pause();
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, PromptToAddCrit);
            }
        }

        private void PromptToAddCrit(object sender, EventArgs e)
        {
            DecisionSubPhase decisionSubPhase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(DecisionSubPhase),
                Triggers.FinishTrigger);

            decisionSubPhase.DescriptionShort = HostShip.PilotInfo.PilotName;
            decisionSubPhase.DescriptionLong = "Would you like to spend 2 Force to add 1 critical result?";
            decisionSubPhase.ImageSource = HostShip;

            decisionSubPhase.AddDecision("Yes", AddCriticalResult);
            decisionSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            decisionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            decisionSubPhase.ShowSkipButton = true;

            decisionSubPhase.DefaultDecisionName = "Yes";

            decisionSubPhase.Start();
        }

        private void AddCriticalResult(object sender, EventArgs e)
        {
            HostShip.State.Force -= 2;
            Combat.DiceRollAttack.AddDice(DieSide.Crit);

            Messages.ShowInfoToHuman(HostShip.PilotInfo.PilotName + ": Critical hit result was added");

            DecisionSubPhase.ConfirmDecision();
            Phases.CurrentSubPhase.Resume();
        }
    }
}