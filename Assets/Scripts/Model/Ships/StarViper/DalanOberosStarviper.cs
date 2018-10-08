using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using Abilities;
using Tokens;
using RuleSets;
using ActionsList;
using SubPhases;

namespace Ship
{
    namespace StarViper
    {
        public class DalanOberosStarviper : StarViper, ISecondEditionPilot
        {
            public DalanOberosStarviper() : base()
            {
                PilotName = "Dalan Oberos";
                PilotSkill = 4;
                Cost = 54;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.DalanOberosStarviperAbility());

                SEImageNumber = 179;
            }

            public void AdaptPilotToSecondEdition()
            {

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
            subphase.InfoText = "Gain Stress to rotate the ship?";

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

