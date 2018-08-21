using System;
using System.Collections.Generic;
using RuleSets;
using Ship;
using SubPhases;
using Tokens;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class CaptainKagi : LambdaShuttle, ISecondEditionPilot
        {
            public CaptainKagi() : base()
            {
                PilotName = "Captain Kagi";
                PilotSkill = 8;
                Cost = 27;
                IsUnique = true;

                PilotAbilities.Add(new Abilities.CaptainKagiAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 48;

                PilotAbilities.RemoveAll(ability => ability is Abilities.CaptainKagiAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.CaptainKagiAbilitySE());
            }
        }
    }
}


namespace Abilities
{
    public class CaptainKagiAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed += CanPerformTargetLock;
        }

        public override void DeactivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed -= CanPerformTargetLock;
        }

        public void CanPerformTargetLock(ref bool result, GenericShip attacker, GenericShip defender)
        {
            bool abilityIsActive = false;
            if (defender.ShipId != HostShip.ShipId)
            {
                if (defender.Owner.PlayerNo == HostShip.Owner.PlayerNo)
                {
                    BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(attacker, HostShip);
                    if (positionInfo.Range >= attacker.TargetLockMinRange && positionInfo.Range <= attacker.TargetLockMaxRange)
                    {
                        abilityIsActive = true;
                    }
                }
            }

            if (abilityIsActive)
            {
                if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                {
                    Messages.ShowErrorToHuman("Captain Kagi: You cannot target lock that ship");
                }
                result = false;
            }
        }

    }
}

namespace Abilities.SecondEdition
{
    public class CaptainKagiAbilitySE : GenericAbility
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
                    true,
                    null,
                    HostShip.PilotName,
                    "Choose a ship to transfer red target lock tokens from.",
                    HostShip.ImageUrl
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
            Actions.ReassignToken(token, TargetShip, HostShip, delegate { ReassignTLTokens(tokens); });
        }
    }
}