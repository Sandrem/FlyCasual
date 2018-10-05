using Abilities.SecondEdition;
using RuleSets;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace UWing
    {
        public class CassianAndor : UWing, ISecondEditionPilot
        {
            public CassianAndor() : base()
            {
                PilotName = "Cassian Andor";
                PilotSkill = 3;
                Cost = 47;

                IsUnique = true;

                PrintedUpgradeIcons.Add(UpgradeType.Elite);
                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new CassianAndorSE());

                SEImageNumber = 56;
            }

            public void AdaptPilotToSecondEdition()
            {
                // not needed
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CassianAndorSE : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, Ability);
        }

        protected virtual string GenerateAbilityMessage()
        {
            return "Choose a ship to remove a stress token from.";
        }

        private void Ability(object sender, EventArgs e)
        {
            bool friendlyShipIsStressed = false;

            foreach(var kv in HostShip.Owner.Ships)
            {
                GenericShip ship = kv.Value;
                if(ship.Tokens.HasToken(typeof(StressToken)))
                {
                    friendlyShipIsStressed = true;
                }
            }

            if (friendlyShipIsStressed)
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    true,
                    null,
                    HostShip.PilotName,
                    GenerateAbilityMessage(),
                    HostShip.ImageUrl
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                ship.Tokens.HasToken(typeof(StressToken));
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            if (Actions.HasTarget(ship)) result += 100;
            return result;
        }

        private void SelectAbilityTarget()
        {
            TargetShip.Tokens.RemoveToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
        }
    }
}