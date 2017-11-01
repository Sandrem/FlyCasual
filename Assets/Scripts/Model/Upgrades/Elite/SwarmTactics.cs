using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class SwarmTactics : GenericUpgrade
    {

        public SwarmTactics() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Swarm Tactics";
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            Host = host;
            base.AttachToShip(host);

            Phases.OnCombatPhaseStart += PlanSwarmTacticsPilotAbility;
        }

        private void PlanSwarmTacticsPilotAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "#" + Host.ShipId + ": Swarm Tactics",
                TriggerOwner = Host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnCombatPhaseStart,
                EventHandler = SwarmTacticsPilotAbility
            });
        }

        private void SwarmTacticsPilotAbility(object sender, System.EventArgs e)
        {
            Selection.ThisShip = Host;
            if (Host.Owner.Ships.Count > 1)
            {
                Phases.StartTemporarySubPhaseOld(
                    "Select target for Swarm Tactics",
                    typeof(SubPhases.SelectSwarmTacticsTargetSubPhase),
                    Triggers.FinishTrigger
                );
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
            isFriendlyAllowed = true;
            maxRange = 1;
            finishAction = SelectSwarmTacticsTarget;

            UI.ShowSkipButton();
        }

        private void SelectSwarmTacticsTarget()
        {
            new SwarmTacticsPilotSkillModifier(TargetShip, Selection.ThisShip.PilotSkill);
            MovementTemplates.ReturnRangeRuler();

            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

        protected override void RevertSubPhase() { }

        private class SwarmTacticsPilotSkillModifier : Ship.IModifyPilotSkill
        {
            private Ship.GenericShip host;
            private int newPilotSkill;

            public SwarmTacticsPilotSkillModifier(Ship.GenericShip host, int newPilotSkill)
            {
                this.host = host;
                this.newPilotSkill = newPilotSkill;

                host.AddPilotSkillModifier(this);
                Phases.OnEndPhaseStart += RemoveSwarmTacticsModifieer;
            }

            public void ModifyPilotSkill(ref int pilotSkill)
            {
                pilotSkill = newPilotSkill;
            }

            private void RemoveSwarmTacticsModifieer()
            {
                host.RemovePilotSkillModifier(this);
            }
        }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
