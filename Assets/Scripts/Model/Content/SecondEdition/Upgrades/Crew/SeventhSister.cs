using Ship;
using Upgrade;
using SubPhases;
using System;
using BoardTools;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class SeventhSister : GenericUpgrade
    {
        public SeventhSister() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Seventh Sister",
                UpgradeType.Crew,
                cost: 9,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                addForce: 1,
                abilityType: typeof(Abilities.SecondEdition.SeventhSisterCrewAbility),
                seImageNumber: 121
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    //If an enemy ship at Range 0-1 would gain a stress token, you may spend 1 force to have it gain 1 jam or tractor token instead.
    public class SeventhSisterCrewAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.BeforeTokenIsAssignedGlobal += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.BeforeTokenIsAssignedGlobal -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, Type tokenType)
        {
            if (tokenType == typeof(Tokens.StressToken) 
                && ship.Owner != HostShip.Owner
                && new DistanceInfo(ship, HostShip).Range <= 1)
            {
                TargetShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnBeforeTokenIsAssigned, ShowDecision);
            }
        }

        private void ShowDecision(object sender, EventArgs e)
        {
            if (HostShip.State.Force > 0)
            {
                var phase = Phases.StartTemporarySubPhaseNew<SeventhSisterDecistionSubPhase>(
                     $"{HostName}: You may spend 1 force to have {TargetShip.PilotInfo.PilotName} gain 1 jam or tractor token instead of stress",
                    Triggers.FinishTrigger);
                phase.TargetShip = TargetShip;
                phase.HostShip = HostShip;
                phase.TokenSelected = TokenSelected;
                phase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void TokenSelected(Tokens.GenericToken token)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.State.Force--;
                TargetShip.Tokens.TokenToAssign = token; 
            }
            DecisionSubPhase.ConfirmDecision();

        }

        protected class SeventhSisterDecistionSubPhase : DecisionSubPhase
        {
            public GenericShip HostShip;
            public GenericShip TargetShip;
            public Action<Tokens.GenericToken> TokenSelected;
            public override void PrepareDecision(Action callBack)
            {
                InfoText = $"Seventh Sister: You may spend 1 force to have {TargetShip.PilotInfo.PilotName} gain 1 jam or tractor token instead of stress";

                DecisionViewType = DecisionViewTypes.TextButtons;

                AddDecision("Jam", delegate { TokenSelected(new Tokens.JamToken(TargetShip)); });
                AddDecision("Tractor", delegate { TokenSelected(new Tokens.TractorBeamToken(TargetShip, HostShip.Owner)); });

                DefaultDecisionName = GetDecisions().First().Name;
                ShowSkipButton = true;
                callBack();
            }
        }
    }
}