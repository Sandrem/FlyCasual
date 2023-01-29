using Actions;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using SubPhases;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class PoeDameron : ScavengedYT1300
        {
            public PoeDameron() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Poe Dameron",
                    "A Difficult Man",
                    Faction.Resistance,
                    6,
                    7,
                    25,
                    isLimited: true,
                    charges: 2,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.PoeDameronYT1300PilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Title,
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    }
                );

                PilotNameCanonical = "poedameron-scavengedyt1300";

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/6d63e9f9-05c3-48c3-bcc1-768e378a5560/SWZ97_PoeDameronlegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PoeDameronYT1300PilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementActivationStart += RegisterTriggerActivationPhase;
            HostShip.OnMovementFinishSuccessfully += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivationStart -= RegisterTriggerActivationPhase;
            HostShip.OnMovementFinishSuccessfully -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (HostShip.State.Charges > 1)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, PerformFreeFocusAction);
            }
        }

        private void PerformFreeFocusAction(object sender, System.EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterSpendChargeTrigger;
            HostShip.OnActionIsPerformed += RegisterExposeDamageCardTrigger;
            HostShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new BoostAction(),
                    new BarrelRollAction() { Color = ActionColor.Red }
                },
                CleanUp,
                HostShip.PilotInfo.PilotName,
                "After you fully execute a maneuver you may spend 2 Charges to perform a white Boost or a red Barrel Roll action. If you perform a red Barrel Roll action, expose 1 damage card if able.",
                HostShip
            );
        }

        private void RegisterExposeDamageCardTrigger(GenericAction action)
        {
            HostShip.OnActionIsPerformed -= RegisterExposeDamageCardTrigger;
            if (action is BarrelRollAction && HostShip.Damage.GetFacedownCards().Count > 0)
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnFreeAction,
                    delegate
                    {
                        Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has performed a red Barrel Roll action and must expose one damage card.");
                        HostShip.Damage.ExposeRandomFacedownCard(Triggers.FinishTrigger);
                    }
                );
            }
        }

        private void RegisterSpendChargeTrigger(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate
                {
                    HostShip.SpendCharges(2);
                    Triggers.FinishTrigger();
                }
            );
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= RegisterSpendChargeTrigger;
            HostShip.OnActionIsPerformed -= RegisterExposeDamageCardTrigger;
            Triggers.FinishTrigger();
        }

        public void RegisterTriggerActivationPhase(GenericShip host)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskToUseAbilityActivationPhase);
            }
        }

        private void AskToUseAbilityActivationPhase(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                TurnOnIgnoreObstaclesActivationPhase,
                descriptionLong: "Do you want to spend 1 Charge to ignore obstacles during this maneuver?",
                imageHolder: HostShip
            );
        }

        private void TurnOnIgnoreObstaclesActivationPhase(object sender, EventArgs e)
        {
            HostShip.SpendCharge();
            HostShip.IsIgnoreObstacles = true;
            HostShip.IsIgnoreObstacleObstructionWhenAttacking = true;
            Phases.Events.OnActivationPhaseEnd_NoTriggers += TurnOffIgnoreObstaclesActivationPhase;
            DecisionSubPhase.ConfirmDecision();
        }

        private void TurnOffIgnoreObstaclesActivationPhase()
        {
            HostShip.IsIgnoreObstacles = false;
            HostShip.IsIgnoreObstacleObstructionWhenAttacking = false;
            Phases.Events.OnActivationPhaseEnd_NoTriggers -= TurnOffIgnoreObstaclesActivationPhase;
        }

    }
}