using ActionsList;
using BoardTools;
using RulesList;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SensorScramblers : GenericUpgrade
    {
        public SensorScramblers() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Sensor Scramblers",
                type: UpgradeType.Tech,
                cost: 1,
                restriction: new ShipRestriction
                (
                    typeof(Ship.SecondEdition.TIEWiWhisperModifiedInterceptor.TIEWiWhisperModifiedInterceptor),
                    typeof(Ship.SecondEdition.TIEVnSilencer.TIEVnSilencer)
                ),
                abilityType: typeof(Abilities.SecondEdition.SensorScramblersAbility)
            );
            
            ImageUrl = "https://i.imgur.com/BJPKpq0.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SensorScramblersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterGainCloakToken;
            TargetLocksRule.OnCheckTargetLockIsDisallowed += ForbidLockOnMe;
            Phases.Events.OnEndPhaseStart_Triggers += CheckForcedDecloak;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterGainCloakToken;
            TargetLocksRule.OnCheckTargetLockIsDisallowed -= ForbidLockOnMe;
            Phases.Events.OnEndPhaseStart_Triggers -= CheckForcedDecloak;
        }

        private void RegisterGainCloakToken()
        {
            RegisterAbilityTrigger(TriggerTypes.OnSetupEnd, GainCloakToken);
        }

        private void GainCloakToken(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} gains Cloak token");
            HostShip.Tokens.AssignToken(typeof(Tokens.CloakToken), Triggers.FinishTrigger);
        }

        private void ForbidLockOnMe(ref bool isAllowed, GenericShip lockSource, ITargetLockable lockTarget)
        {
            if (lockTarget is GenericShip && (lockTarget as GenericShip) == HostShip
                && HostShip.IsCloaked)
            {
                isAllowed = false;
            }
        }

        private void CheckForcedDecloak()
        {
            if (HostShip.IsCloaked
                && Board.GetShipsAtRange(HostShip, new Vector2(0,3), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, PerfromForcedDecloak);
            }
        }

        private void PerfromForcedDecloak(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} must decloak");

            Selection.ChangeActiveShip(HostShip);

            HostShip.OnActionIsReallyFailed += RegisterDiscardCloakToken;
            HostShip.OnPositionFinish += ClearForcedCloakDiscard;

            Phases.StartTemporarySubPhaseOld(
                "Decloak",
                typeof(DecloakPlanningSubPhase),
                delegate
                {
                    Phases.FinishSubPhase(typeof(DecloakPlanningSubPhase));
                    Triggers.FinishTrigger();
                }
            );
        }

        private void ClearForcedCloakDiscard(GenericShip ship)
        {
            HostShip.OnActionIsReallyFailed -= RegisterDiscardCloakToken;
            HostShip.OnPositionFinish -= ClearForcedCloakDiscard;
        }

        private void RegisterDiscardCloakToken(GenericAction action)
        {
            RegisterAbilityTrigger(TriggerTypes.OnActionIsReallyFailed, DiscardCloakToken);
        }

        private void DiscardCloakToken(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: Decloak is failed, {HostShip.PilotInfo.PilotName} lost Cloak token instead");

            ClearForcedCloakDiscard(HostShip);
            HostShip.Tokens.RemoveToken(
                typeof(Tokens.CloakToken),
                delegate
                {
                    Phases.FinishSubPhase(typeof(DecloakPlanningSubPhase));
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}