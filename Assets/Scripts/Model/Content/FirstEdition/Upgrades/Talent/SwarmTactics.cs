using Upgrade;
using System.Collections.Generic;
using Ship;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class SwarmTactics : GenericUpgrade
    {
        public SwarmTactics() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Swarm Tactics",
                UpgradeType.Talent,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.SwarmTacticsAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SwarmTacticsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += PlanSwarmTacticsPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= PlanSwarmTacticsPilotAbility;
        }

        private void PlanSwarmTacticsPilotAbility()
        {
            var trigger = RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, SwarmTacticsPilotAbility);
            trigger.Name = $"{HostName} - {HostShip.PilotInfo.PilotName} (#{HostShip.ShipId})";
        }

        private void SwarmTacticsPilotAbility(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                SelectSwarmTacticsTarget,
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "Select target for Swarm Tactics",
                HostUpgrade
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, ship);
            return (distanceInfo.Range == 1) && (ship.Owner == HostShip.Owner) && (ship.ShipId != HostShip.ShipId);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            if (ActionsHolder.HasTarget(ship)) result += 100;
            result += (12 - ship.State.Initiative);

            if (ship.State.Initiative >= HostShip.State.Initiative) result = 0;

            return result;
        }

        private void SelectSwarmTacticsTarget()
        {
            new SwarmTacticsPilotSkillModifier(TargetShip, HostShip.State.Initiative);
            MovementTemplates.ReturnRangeRuler();

            SelectShipSubPhase.FinishSelection();
        }

        private class SwarmTacticsPilotSkillModifier : IModifyPilotSkill
        {
            private GenericShip host;
            private int newPilotSkill;

            public SwarmTacticsPilotSkillModifier(GenericShip host, int newPilotSkill)
            {
                this.host = host;
                this.newPilotSkill = newPilotSkill;

                host.State.AddPilotSkillModifier(this);
                Phases.Events.OnEndPhaseStart_NoTriggers += RemoveSwarmTacticsModifieer;
            }

            public void ModifyPilotSkill(ref int pilotSkill)
            {
                pilotSkill = newPilotSkill;
            }

            private void RemoveSwarmTacticsModifieer()
            {
                host.State.RemovePilotSkillModifier(this);

                Phases.Events.OnEndPhaseStart_NoTriggers -= RemoveSwarmTacticsModifieer;
            }
        }
    }
}
