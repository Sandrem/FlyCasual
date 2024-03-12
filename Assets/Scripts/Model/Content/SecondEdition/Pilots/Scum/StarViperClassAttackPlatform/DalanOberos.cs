using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class DalanOberos : StarViperClassAttackPlatform
        {
            public DalanOberos() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Dalan Oberos",
                    "Elite Bounty Hunter",
                    Faction.Scum,
                    4,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DalanOberosStarviperAbility),
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Torpedo,
                        UpgradeType.Modification
                    },
                    seImageNumber: 179,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                PilotNameCanonical = "dalanoberos-starviperclassattackplatform";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DalanOberosStarviperAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += AskToUseDalanOberosStarviperAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= AskToUseDalanOberosStarviperAbility;
        }

        private void AskToUseDalanOberosStarviperAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToRotate);
        }

        private void AskToRotate(object sender, EventArgs e)
        {
            DalanOberosStarviperDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<DalanOberosStarviperDecisionSubphase>("Rotate the ship?", Triggers.FinishTrigger);

            subphase.DecisionOwner = HostShip.Owner;

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Do you want to gain a Stress Token to rotate the ship?";
            subphase.ImageSource = HostShip;

            subphase.AddDecision("90 Counterclockwise", Rotate90Counterclockwise);
            subphase.AddDecision("90 Clockwise", Rotate90Clockwise);
            subphase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

            subphase.DefaultDecisionName = "No";

            subphase.Start();
        }

        private void Rotate90Clockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Tokens.AssignToken(
                typeof(StressToken),
                delegate { HostShip.Rotate90Clockwise(Triggers.FinishTrigger); }
            );
        }

        private void Rotate90Counterclockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Tokens.AssignToken(
                typeof(StressToken),
                delegate { HostShip.Rotate90Counterclockwise(Triggers.FinishTrigger); }
            );
        }

        private class DalanOberosStarviperDecisionSubphase : DecisionSubPhase { };
    }
}