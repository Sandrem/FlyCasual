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
            Type = UpgradeSlot.Elite;
            Name = ShortName = "Swarm Tactics";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/7/75/Swarm_Tactics.png";
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
            Phases.StartTemporarySubPhase(
                "Select target for Swarm Tactics",
                typeof(SubPhases.SelectSwarmTacticsTargetSubPhase),
                Triggers.FinishTrigger
            );
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

            Game.UI.ShowSkipButton();
        }

        private void SelectSwarmTacticsTarget()
        {
            new SwarmTacticsPilotSkillModifier(TargetShip, Selection.ThisShip.PilotSkill);
            MovementTemplates.ReturnRangeRuler();
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

    }

}
