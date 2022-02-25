using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class RoarkGarnet : Hwk290LightFreighter
        {
            public RoarkGarnet() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Roark Garnet",
                    "Good-Hearted Smuggler",
                    Faction.Rebel,
                    4,
                    6,
                    15,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RoarkGarnetAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    seImageNumber: 44
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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
            return "Choose another friendly ship in arc.\nUntil the end of the phase, treat that ship's pilot skill value as \"7\".";
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
                    HostShip.PilotInfo.PilotName,
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
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                Board.IsShipInArc(HostShip, ship);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            if (ActionsHolder.HasTarget(ship)) result += 100;
            result += (12 - ship.State.Initiative);
            return result;
        }

        private void SelectAbilityTarget()
        {
            TargetShip.State.AddPilotSkillModifier(this);
            Phases.Events.OnEndPhaseStart_NoTriggers += RemovePilotSkillModifieer;
            SelectShipSubPhase.FinishSelection();
        }

        public virtual void RemovePilotSkillModifieer()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers -= RemovePilotSkillModifieer;
            TargetShip.State.RemovePilotSkillModifier(this);
        }

        public virtual void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 7;
        }
    }
}
