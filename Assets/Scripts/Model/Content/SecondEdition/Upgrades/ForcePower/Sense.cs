using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Sense : GenericUpgrade
    {
        public Sense() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Sense",
                UpgradeType.ForcePower,
                cost: 6,
                abilityType: typeof(Abilities.SecondEdition.SenseAbility),
                seImageNumber: 21,
                legalityInfo: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                }
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class SenseAbility : Abilities.FirstEdition.IntelligenceAgentAbility
    {
        protected override int MinRange { get { return 0; } }
        protected override int MaxRange { get { return (HostShip.State.Force > 0) ? 3 : 1; } }

        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += RegisterAbilityTriggerByShip;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= RegisterAbilityTriggerByShip;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(MinRange, MaxRange), Team.Type.Enemy).Count > 0 &&
                HostShip.State.Force > 0 &&
                HostShip.IsCanUseForceNow()
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, StartSelectTargetForAbility);
            }
        }

        private void RegisterAbilityTriggerByShip(GenericShip ship, ref bool flag)
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(MinRange, MaxRange), Team.Type.Enemy).Count > 0 &&
                HostShip.State.Force > 0 &&
                HostShip.IsCanUseForceNow()
            )
            {
                flag = true;
            }
        }

        protected override void SeeAssignedManuver()
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, TargetShip);
            if (distInfo.Range > 1)
            {
                HostShip.State.SpendForce(1, base.SeeAssignedManuver);
            }
            else
            {
                base.SeeAssignedManuver();
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class IntelligenceAgentAbility : GenericAbility
    {
        protected virtual int MinRange { get { return 1; } }
        protected virtual int MaxRange { get { return 2; } }

        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += RegisterAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= RegisterAbilityTrigger;
        }

        private void RegisterAbilityTrigger()
        {
            if (Board.GetShipsAtRange(HostShip, new Vector2(MinRange, MaxRange), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskToUseIntelligenceAgentAbility);
            }
        }

        private void AskToUseIntelligenceAgentAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                delegate {
                    DecisionSubPhase.ConfirmDecisionNoCallback();
                    StartSelectTargetForAbility(sender, e);
                },
                descriptionLong: "Do you want to choose 1 enemy ship at Range 1-2 to look at that ship's chosen maneuver?",
                imageHolder: HostUpgrade
            );
        }

        protected void StartSelectTargetForAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                SeeAssignedManuver,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "Choose a ship to look at it's chosen maneuver",
                HostUpgrade
            );
        }

        protected virtual void SeeAssignedManuver()
        {
            if (TargetShip.Owner is Players.HotacAiPlayer)
            {
                Messages.ShowErrorToHuman("This ability is useless against HotAC AI =)");
            }
            else
            {
                Roster.ToggleManeuverVisibility(TargetShip, true);
                TargetShip.AlwaysShowAssignedManeuver = true;
            }

            SelectShipSubPhase.FinishSelection();
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy })
                && FilterTargetsByRange(ship, MinRange, MaxRange)
                && ship.AssignedManeuver != null;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }
    }
}