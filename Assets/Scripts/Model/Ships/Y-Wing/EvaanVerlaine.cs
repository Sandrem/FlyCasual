using Abilities.SecondEdition;
using RuleSets;
using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace YWing
    {
        public class EvaanVerlaine : YWing, ISecondEditionPilot
        {
            public EvaanVerlaine() : base()
            {
                PilotName = "Evaan Verlaine";
                PilotSkill = 3;
                Cost = 36;

                IsUnique = true;

                faction = Faction.Rebel;
                PilotRuleType = typeof(SecondEdition);
                PrintedUpgradeIcons.Add(UpgradeType.Elite);
                PilotAbilities.Add(new EvaanVerlaineAbilitySE());

                SEImageNumber = 16;
            }

            public void AdaptPilotToSecondEdition()
            {
                // empty unneeded bam
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EvaanVerlaineAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += TryRegisterEvaanVerlaineAbiliity;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= TryRegisterEvaanVerlaineAbiliity;
        }

        private void TryRegisterEvaanVerlaineAbiliity()
        {
            if (TargetsForAbilityExist(FilterAbilityTargets) && HostShip.Tokens.HasToken<FocusToken>())
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskSelectShip);
            }
        }

        private void AskSelectShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                delegate
                {
                    HostShip.Tokens.SpendToken(typeof(FocusToken), IncreaseAgilityValue);
                },
                FilterAbilityTargets,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotName,
                "Choose a ship to increase it's agility value.",
                HostShip
            );
        }

        private bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This }) && FilterTargetsByRange(ship, 1, 1);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            // I don't know how good of a metric this is?
            return ship.Cost * ship.Agility;
        }

        protected void IncreaseAgilityValue()
        {
            TargetShip.ChangeAgilityBy(+1);
            Phases.Events.OnEndPhaseStart_NoTriggers += DecreaseAgilityValue;
            SelectShipSubPhase.FinishSelection();
        }

        protected void DecreaseAgilityValue()
        {
            TargetShip.ChangeAgilityBy(-1);
            Phases.Events.OnEndPhaseStart_NoTriggers -= DecreaseAgilityValue;
        }
    }
}
