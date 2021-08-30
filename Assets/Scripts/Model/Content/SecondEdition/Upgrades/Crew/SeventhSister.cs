using Ship;
using Upgrade;
using SubPhases;
using System;
using BoardTools;
using System.Linq;
using UnityEngine;
using Tokens;

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

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(479, 6)
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

        private void RegisterAbility(GenericShip ship, GenericToken token)
        {
            if (token is StressToken
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
                phase.DecisionOwner = HostShip.Owner;
                phase.TargetShip = TargetShip;
                phase.HostShip = HostShip;
                phase.TokenSelected = TokenSelected;
                phase.SourceUpgrade = HostUpgrade;
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
                TargetShip.Tokens.TokenToAssign = token;
                HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
            }
            else
            {
                DecisionSubPhase.ConfirmDecision();
            }
        }

        protected class SeventhSisterDecistionSubPhase : DecisionSubPhase
        {
            public GenericShip HostShip;
            public GenericShip TargetShip;
            public Action<Tokens.GenericToken> TokenSelected;
            public GenericUpgrade SourceUpgrade;

            public override void PrepareDecision(Action callBack)
            {
                DescriptionShort = "Seventh Sister";
                DescriptionLong = $"You may spend 1 force to have {TargetShip.PilotInfo.PilotName} gain 1 jam or tractor token instead of stress";
                ImageSource = SourceUpgrade;

                DecisionViewType = DecisionViewTypes.TextButtons;

                AddDecision("Jam", delegate { TokenSelected(new Tokens.JamToken(TargetShip, HostShip.Owner)); });
                AddDecision("Tractor", delegate { TokenSelected(new Tokens.TractorBeamToken(TargetShip, HostShip.Owner)); });

                DefaultDecisionName = GetDecisions().First().Name;
                ShowSkipButton = true;
                callBack();
            }
        }
    }
}