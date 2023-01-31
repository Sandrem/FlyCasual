using Ship;
using SubPhases;
using System;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class JynErso : GenericUpgrade
    {
        public JynErso() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Jyn Erso",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.JynErsoCrewAbility),
                seImageNumber: 85
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(426, 18),
                new Vector2(125, 125)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class JynErsoCrewAbility : GenericAbility
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

        private void RegisterJanOrsCrewAbility(GenericShip ship, GenericToken token)
        {
            if (token is FocusToken
                && ship.Owner == HostShip.Owner
                && !IsAbilityUsed)
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
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                UseJanOrsAbility,
                descriptionLong: "Do you want to assign an evade token instead?",
                imageHolder: HostUpgrade
            );
        }

        private void UseJanOrsAbility(object sender, System.EventArgs e)
        {
            TargetShip.Tokens.AssignToken(typeof(EvadeToken), delegate
            {
                TargetShip.Tokens.TokenToAssign = null;
                TargetShip = null;
                DecisionSubPhase.ConfirmDecision();
            });
        }
    }
}