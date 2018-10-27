using Ship;
using System.Collections.Generic;
using System;
using SubPhases;
using RuleSets;
using BoardTools;
using Arcs;

namespace Ship
{
    namespace HWK290
    {
        public class RoarkGarnet : HWK290, ISecondEditionPilot
        {
            public RoarkGarnet() : base()
            {
                PilotName = "Roark Garnet";
                PilotSkill = 4;
                Cost = 19;

                IsUnique = true;

                faction = Faction.Rebel;

                PilotAbilities.Add(new Abilities.RoarkGarnetAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 38;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.RemoveAll(ability => ability is Abilities.RoarkGarnetAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.RoarkGarnetAbilitySE());

                SEImageNumber = 44;
            }
        }
    }
}

namespace Abilities
{
    public class RoarkGarnetAbility : GenericAbility, IModifyPilotSkill
    {
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
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        protected virtual string GenerateAbilityMessage()
        {
            return "Choose another friendly ship in arc.\nUntil the end of the phase, treat that ship's pilot skill value as \"12\".";
        }

        private void Ability(object sender, EventArgs e)
        {
            if (HostShip.Owner.Ships.Count > 1)
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotName,
                    GenerateAbilityMessage(),
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            if (Actions.HasTarget(ship)) result += 100;
            result += (12 - ship.PilotSkill);
            return result;
        }

        private void SelectAbilityTarget()
        {
            TargetShip.AddPilotSkillModifier(this);
            Phases.Events.OnEndPhaseStart_NoTriggers += RemovePilotSkillModifieer;
            SelectShipSubPhase.FinishSelection();
        }

        private void RemovePilotSkillModifieer()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers -= RemovePilotSkillModifieer;
            TargetShip.RemovePilotSkillModifier(this);
        }

        public virtual void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 12;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RoarkGarnetAbilitySE : RoarkGarnetAbility
    {
        protected override string GenerateAbilityMessage()
        {
            return "Choose another friendly ship.\nUntil the end of the phase, treat that ship's pilot skill value as \"7\".";
        }

        public override void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 7;
        }

        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                Board.IsShipInArc(HostShip, ship);
        }
    }
}
