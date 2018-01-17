using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using Abilities;

namespace UpgradesList
{
    public class SwarmTactics : GenericUpgrade
    {

        public SwarmTactics() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Swarm Tactics";
            Cost = 2;

            UpgradeAbilities.Add(new SwarmTacticsAbility());
        }
    }
}

namespace Abilities
{
    public class SwarmTacticsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart += PlanSwarmTacticsPilotAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart -= PlanSwarmTacticsPilotAbility;
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
            targetsAllowed.Add(TargetTypes.OtherFriendly);
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
