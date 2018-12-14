using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LambdaClassT4AShuttle
    {
        public class CaptainKagi : LambdaClassT4AShuttle
        {
            public CaptainKagi() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Kagi",
                    4,
                    48,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainKagiAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 142
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainKagiAbility : GenericAbility
    {
        List<GenericShip> currentTargets;

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            currentTargets = new List<GenericShip>();
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskAbility);
        }

        private void AskAbility(object sender, EventArgs e)
        {
            if (TargetsForAbilityExist(FilterAbilityTarget))
            {
                Messages.ShowInfoToHuman("Captain Kagi: Select a target for his ability.");

                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "Choose a ship to transfer red target lock tokens from.",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool FilterAbilityTarget(GenericShip ship)
        {
            if (currentTargets.Contains(ship))
                return false;

            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3) && FilterTargetWithTokens(ship);
        }

        private bool FilterTargetWithTokens(GenericShip ship)
        {
            return ship.Tokens.HasToken<RedTargetLockToken>('*');
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            return 100;
        }

        private void SelectAbilityTarget()
        {
            currentTargets.Add(TargetShip);
            List<RedTargetLockToken> redTLTokens = TargetShip.Tokens.GetTokens<RedTargetLockToken>('*');
            ReassignTLTokens(redTLTokens);
        }

        private void ReassignTLTokens(List<RedTargetLockToken> tokens)
        {
            if (tokens.Count == 0)
            {
                SelectShipSubPhase.FinishSelectionNoCallback();
                AskAbility(HostShip, new EventArgs()); // weird hack gotta be a better way of doing this
                return;
            }

            RedTargetLockToken token = tokens[0];
            tokens.RemoveAt(0);
            ActionsHolder.ReassignToken(token, TargetShip, HostShip, delegate { ReassignTLTokens(tokens); });
        }
    }
}
