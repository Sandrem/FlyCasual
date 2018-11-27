using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class SwarmTactics : GenericUpgrade
    {
        public SwarmTactics() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Swarm Tactics",
                UpgradeType.Elite,
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
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "#" + HostShip.ShipId + ": Swarm Tactics",
                TriggerOwner = HostShip.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnCombatPhaseStart,
                EventHandler = SwarmTacticsPilotAbility
            });
        }

        private void SwarmTacticsPilotAbility(object sender, System.EventArgs e)
        {
            Selection.ThisShip = HostShip;
            if (HostShip.Owner.Ships.Count > 1)
            {
                var phase = Phases.StartTemporarySubPhaseNew<SubPhases.SelectSwarmTacticsTargetSubPhase>(
                    "Select target for Swarm Tactics",
                    Triggers.FinishTrigger
                );

                phase.ImageSource = HostUpgrade;
                phase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

    }

}

namespace SubPhases
{

    public class SelectSwarmTacticsTargetSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            targetsAllowed.Add(TargetTypes.OtherFriendly);
            maxRange = 1;
            finishAction = SelectSwarmTacticsTarget;

            FilterTargets = FilterAbilityTargets;
            GetAiPriority = GetAiAbilityPriority;

            Description = "Select target for Swarm Tactics";

            RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            UI.ShowSkipButton();
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Selection.ThisShip, ship);
            return (distanceInfo.Range == 1) && (ship.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo) && (ship.ShipId != Selection.ThisShip.ShipId);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            if (ActionsHolder.HasTarget(ship)) result += 100;
            result += (12 - ship.State.Initiative);

            if (ship.State.Initiative >= Selection.ThisShip.State.Initiative) result = 0;

            return result;
        }

        private void SelectSwarmTacticsTarget()
        {
            new SwarmTacticsPilotSkillModifier(TargetShip, Selection.ThisShip.State.Initiative);
            MovementTemplates.ReturnRangeRuler();

            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

        public override void RevertSubPhase() { }

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

        public override void SkipButton()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
