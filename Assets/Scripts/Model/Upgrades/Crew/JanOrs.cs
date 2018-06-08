using Ship;
using Upgrade;
using System;
using SubPhases;
using UpgradesList;
using Tokens;
using Abilities;
using UnityEngine;

namespace UpgradesList
{
    public class JanOrs : GenericUpgrade
    {
        public bool IsAbilityUsed;

        public JanOrs() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Jan Ors";
            Cost = 2;

            isUnique = true;

            UpgradeAbilities.Add(new JanOrsCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{

    // SE Jyn Erso has similar ability, but is not round limited (so don't set IsAbilityUsed to true)
    public class JanOrsCrewAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            GenericShip.BeforeTokenIsAssignedGlobal += RegisterJanOrsCrewAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            GenericShip.BeforeTokenIsAssignedGlobal -= RegisterJanOrsCrewAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void RegisterJanOrsCrewAbility(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.FocusToken) && ship.Owner == HostShip.Owner && !IsAbilityUsed)
            {

                BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(ship, HostShip);
                if (positionInfo.Range <= 3)
                {
                    TargetShip = ship;
                    RegisterAbilityTrigger(TriggerTypes.OnBeforeTokenIsAssigned, ShowDecision);
                }
            }
        }

        private void ShowDecision(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, UseJanOrsAbility);
        }

        private void UseJanOrsAbility(object sender, System.EventArgs e)
        {
            TargetShip.Tokens.AssignToken(new Tokens.EvadeToken(TargetShip), delegate
            {
                TargetShip.Tokens.TokenToAssign = null;
                TargetShip = null;
                IsAbilityUsed = true;
                DecisionSubPhase.ConfirmDecision();
            });
        }
    }
}