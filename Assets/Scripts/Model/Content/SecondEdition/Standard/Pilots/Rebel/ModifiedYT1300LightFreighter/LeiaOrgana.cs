using Content;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ModifiedYT1300LightFreighter
    {
        public class LeiaOrgana : ModifiedYT1300LightFreighter
        {
            public LeiaOrgana() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Leia Organa",
                    "There Is Another",
                    Faction.Rebel,
                    5,
                    8,
                    24,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LeiaOrganaPilotAbility),
                    force: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Title,
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300,
                        Tags.LightSide
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5b/ff/5bffb367-cb6e-4b8b-948f-25a70acd3a3f/swz66_leia-organa.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LeiaOrganaPilotAbility : GenericAbility
    {
        //After a friendly ship fully executes a red maneuver, if it is at range 0-3, you may spend 1 force.
        //If you do, that ship gains 1 focus token or recovers 1 force
        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishSuccessfullyGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishSuccessfullyGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.Owner == HostShip.Owner && ship.GetLastManeuverColor() == MovementComplexity.Complex)
            {
                TargetShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskAbility);
            }
        }

        private void AskAbility(object sender, EventArgs e)
        {
            var isFriendlyInRange = FilterByTargetType(TargetShip, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This }) && FilterTargetsByRange(TargetShip, 0, 3);

            if (isFriendlyInRange && HostShip.State.Force > 0)
            {
                if (TargetShip.State.MaxForce > 0 && TargetShip.State.Force < TargetShip.State.MaxForce)
                {
                    DecisionSubPhase phase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                        Name,
                        typeof(DecisionSubPhase),
                        SelectShipSubPhase.FinishSelection
                    );

                    phase.DescriptionShort = "Spend 1 force to let " + TargetShip.PilotInfo.PilotName + " gain 1 focus or recover 1 force?";
                    phase.RequiredPlayer = HostShip.Owner.PlayerNo;
                    phase.ShowSkipButton = true;

                    phase.AddDecision("Focus", GainFocus);
                    phase.AddDecision("Force", RecoverForce);

                    phase.DefaultDecisionName = "Focus";
                    phase.Start();
                }
                else
                {
                    AskToUseAbility(
                       HostShip.PilotInfo.PilotName,
                       ShouldUseAbility,
                       GainFocus,                       
                       descriptionLong: "Spend 1 force to let " + TargetShip.PilotInfo.PilotName + " gain 1 focus token?",
                       imageHolder: HostShip
                   );
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
               
        }

        private bool ShouldUseAbility()
        {
            return TargetShip.Tokens.CountTokensByType<Tokens.FocusToken>() == 0;
        }

        private void RecoverForce(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": " + TargetShip.PilotInfo.PilotName + " recovers 1 force");
            DecisionSubPhase.ConfirmDecisionNoCallback();
            TargetShip.State.RestoreForce();
            TargetShip = null;
            HostShip.State.SpendForce(1, Triggers.FinishTrigger);
        }

        private void GainFocus(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": " + TargetShip.PilotInfo.PilotName + " gains 1 focus token");
            DecisionSubPhase.ConfirmDecisionNoCallback();
            TargetShip.Tokens.AssignToken(
                typeof(Tokens.FocusToken),
                delegate {
                    TargetShip = null;
                    HostShip.State.SpendForce(1, Triggers.FinishTrigger);
                }
            );
        }
    }
}