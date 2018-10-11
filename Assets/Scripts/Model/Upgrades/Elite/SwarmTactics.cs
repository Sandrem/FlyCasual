using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Abilities;
using RuleSets;

namespace UpgradesList
{
    public class SwarmTactics : GenericUpgrade, ISecondEditionUpgrade
    {

        public SwarmTactics() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Swarm Tactics";
            Cost = 2;

            UpgradeAbilities.Add(new SwarmTacticsAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 3;

            SEImageNumber = 17;
        }
    }
}

namespace Abilities
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
            if (Actions.HasTarget(ship)) result += 100;
            result += (12 - ship.PilotSkill);

            if (ship.PilotSkill >= Selection.ThisShip.PilotSkill) result = 0;

            return result;
        }

        private void SelectSwarmTacticsTarget()
        {
            new SwarmTacticsPilotSkillModifier(TargetShip, Selection.ThisShip.PilotSkill);
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

                host.AddPilotSkillModifier(this);
                Phases.Events.OnEndPhaseStart_NoTriggers += RemoveSwarmTacticsModifieer;
            }

            public void ModifyPilotSkill(ref int pilotSkill)
            {
                pilotSkill = newPilotSkill;
            }

            private void RemoveSwarmTacticsModifieer()
            {
                host.RemovePilotSkillModifier(this);

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
