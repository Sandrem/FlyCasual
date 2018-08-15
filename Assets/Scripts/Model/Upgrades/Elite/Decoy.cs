using Abilities;
using Board;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class Decoy : GenericUpgrade
    {
        public Decoy() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Decoy";
            Cost = 1;

            UpgradeAbilities.Add(new DecoyAbility());
        }
    }
}

namespace Abilities
{
    //At the start of the Combat phase, you may choose 1 friendly ship at Range 1-2.
    //Exchange your pilot skill with that ship's pilot skill until end of the phase.
    public class DecoyAbility : GenericAbility
    {
        FriendlyPilotSkillModifier _hostShip;
        FriendlyPilotSkillModifier _friendlyShip;

        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart += CheckTrigger;

            Phases.OnCombatPhaseEnd += RegisterPilotSkillReset;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart -= CheckTrigger;

            Phases.OnCombatPhaseEnd -= RegisterPilotSkillReset;
        }

        private void CheckTrigger()
        {
            List<GenericShip> shipsInRange = BoardManager.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Friendly);
            shipsInRange.Remove(HostShip);

            if (shipsInRange.Count <= 0)
            {
                return;
            }

            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, delegate { AskToSwapPilotSkills(shipsInRange); });
        }

        private void RegisterPilotSkillReset()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, ResetPilotSkills);
        }

        private void ResetPilotSkills(object sender, EventArgs e)
        {
            _hostShip.RemoveModifier();
            _friendlyShip.RemoveModifier();

            _friendlyShip = null;
            _hostShip = null;

            Triggers.FinishTrigger();
        }

        private void AskToSwapPilotSkills(List<GenericShip> shipsInRange)
        {
            SelectTargetForAbility(SwitchPilotSkills, new List<TargetTypes> { TargetTypes.OtherFriendly }, new Vector2(1, 2), Triggers.FinishTrigger);
        }

        private void SwitchPilotSkills()
        {
            _hostShip = new FriendlyPilotSkillModifier(HostShip, TargetShip.PilotSkill);
            _friendlyShip = new FriendlyPilotSkillModifier(TargetShip, HostShip.PilotSkill);

            _hostShip.AddPilotSkill();
            _friendlyShip.AddPilotSkill();

            Messages.ShowInfoToHuman(string.Format("Swapped pilot skills of {0} and {1}",
                HostShip.PilotName, TargetShip.PilotName));

            DecisionSubPhase.ConfirmDecision();
        }

        private class FriendlyPilotSkillModifier : IModifyPilotSkill
        {
            private GenericShip _host;
            private int _newPilotSkill;

            public FriendlyPilotSkillModifier(GenericShip host, int newPilotSkill)
            {
                _host = host;
                _newPilotSkill = newPilotSkill;
            }

            public void AddPilotSkill()
            {
                _host.AddPilotSkillModifier(this);
            }

            public void ModifyPilotSkill(ref int pilotSkill)
            {
                pilotSkill = _newPilotSkill;
            }

            public void RemoveModifier()
            {
                _host.RemovePilotSkillModifier(this);
            }
        }
    }    
}