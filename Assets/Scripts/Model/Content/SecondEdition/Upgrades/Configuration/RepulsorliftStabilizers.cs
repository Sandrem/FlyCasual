using Upgrade;
using Ship;
using System;
using SubPhases;
using System.Collections.Generic;
using Movement;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class RepulsorliftStabilizersInactive : GenericDualUpgrade
    {
        public RepulsorliftStabilizersInactive() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Repulsorlift Stabilizers (Inactive)",
                UpgradeType.Configuration,
                cost: 3,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.HMPDroidGunship.HMPDroidGunship)),
                abilityType: typeof(Abilities.SecondEdition.RepulsorliftStabilizersInactiveAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/28/39/2839eb69-5a45-47e1-a69d-4bb0b4f8ab5d/swz71_upgrade_stabilizer-inactive.png";

            SelectSideOnSetup = false;
            AnotherSide = typeof(RepulsorliftStabilizersActive);
        }
    }

    public class RepulsorliftStabilizersActive : GenericDualUpgrade
    {
        public RepulsorliftStabilizersActive() : base()
        {
            IsHidden = true;

            NameCanonical = "repulsorliftstabilizers-anotherside";

            UpgradeInfo = new UpgradeCardInfo(
                "Repulsorlift Stabilizers (Active)",
                UpgradeType.Configuration,
                cost: 3,
                //restriction: new ShipRestriction(typeof(Ship.SecondEdition.HMPDroidGunship.HMPDroidGunship)),
                abilityType: typeof(Abilities.SecondEdition.RepulsorliftStabilizersActiveAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b9/24/b92420be-2835-4e12-b76e-b2675533249c/swz71_upgrade_stabilizer-active.png";

            IsSecondSide = true;
            AnotherSide = typeof(RepulsorliftStabilizersInactive);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RepulsorliftStabilizersInactiveAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += ApplyAbility;
            HostShip.OnMovementFinishSuccessfully += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= ApplyAbility;
            HostShip.OnMovementFinishSuccessfully -= RegisterTrigger;
        }

        private void ApplyAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.Bearing == ManeuverBearing.Straight)
            {
                movement.ColorComplexity = GenericMovement.ReduceComplexity(movement.ColorComplexity);
                // Update revealed dial in UI
                Roster.UpdateAssignedManeuverDial(HostShip, HostShip.AssignedManeuver);
                Messages.ShowInfoToHuman("Repulsorlift Stabilizers (Active): Difficulty of straight maneuvers is reduced");
            }
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToFlip);
        }

        private void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                DoFlipSide,
                descriptionLong: "Do you want to flip this card?",
                imageHolder: HostUpgrade
            );
        }

        protected void DoFlipSide(object sender, EventArgs e)
        {
            (HostUpgrade as GenericDualUpgrade).Flip();
            DecisionSubPhase.ConfirmDecision();
        }
    }

    public class RepulsorliftStabilizersActiveAbility : CombinedAbility
    {
        public override List<Type> CombinedAbilities => new List<Type>()
        {
            typeof(RepulsorliftStabilizersActiveSideslipAbility),
            typeof(RepulsorliftStabilizersActiveFlipAbility),
            typeof(RepulsorliftStabilizersActiveMandatoryFlipAbility)
        };

        private class RepulsorliftStabilizersActiveSideslipAbility : TriggeredAbility
        {
            public override TriggerForAbility Trigger => new AfterYourRevealManeuver
            (
                ifBank: true,
                ifTurn: true
            );

            public override AbilityPart Action => new ChangeManeuverAction
            (
                changeToSideslip: true
            );
        }

        private class RepulsorliftStabilizersActiveFlipAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                HostShip.OnMovementFinishSuccessfully += RegisterTrigger;
            }

            public override void DeactivateAbility()
            {
                HostShip.OnMovementFinishSuccessfully -= RegisterTrigger;
            }

            private void RegisterTrigger(GenericShip ship)
            {
                if (HostShip.AssignedManeuver.Bearing != ManeuverBearing.SideslipBank
                    && HostShip.AssignedManeuver.Bearing != ManeuverBearing.SideslipTurn)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToFlip);
                }
            }

            private void AskToFlip(object sender, EventArgs e)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    NeverUseByDefault,
                    DoFlipSide,
                    descriptionLong: "Do you want to flip this card?",
                    imageHolder: HostUpgrade
                );
            }

            protected void DoFlipSide(object sender, EventArgs e)
            {
                (HostUpgrade as GenericDualUpgrade).Flip();
                DecisionSubPhase.ConfirmDecision();
            }
        }

        private class RepulsorliftStabilizersActiveMandatoryFlipAbility : TriggeredAbility
        {
            public override TriggerForAbility Trigger => new AfterManeuver
            (
                onlyIfBearing: ManeuverBearing.SideslipAny
            );

            public override AbilityPart Action => new FlipCardAction
            (
                GetThisUpgrade
            );

            private GenericDualUpgrade GetThisUpgrade()
            {
                return HostUpgrade as GenericDualUpgrade;
            }
        }
    }
}
