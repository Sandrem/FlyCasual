using Ship;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;
using BoardTools;
using Tokens;

namespace UpgradesList
{
    public class HeightenedPerception : GenericUpgrade, ISecondEditionUpgrade, IModifyPilotSkill
    {
        public HeightenedPerception() : base()
        {
            Types.Add(UpgradeType.Force);
            Name = "Heightened Perception";
            Cost = 3;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.HeightenedPerception());

            SEImageNumber = 19;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            // Not required
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 7;
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class HeightenedPerception : GenericAbility
        {
            public override void ActivateAbility()
            {
                Phases.Events.OnCombatPhaseStart_Triggers += RegisterTrigger;
            }

            public override void DeactivateAbility()
            {
                Phases.Events.OnCombatPhaseStart_Triggers -= RegisterTrigger;
            }

            private void RegisterTrigger()
            {
                if (HostShip.Tokens.CountTokensByType(typeof(ForceToken)) > 0)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToUseHeightenedPerception);
                }
            }

            private void AskToUseHeightenedPerception(object sender, System.EventArgs e)
            {
                if (HostShip.Tokens.CountTokensByType(typeof(ForceToken)) > 0)
                {
                    Selection.ChangeActiveShip(HostShip);

                    AskToUseAbility(ShouldUseAbility, UseAbility);
                }
                else
                {
                    Triggers.FinishTrigger();
                }
            }

            private bool ShouldUseAbility()
            {
                return false;
            }

            private void UseAbility(object sender, System.EventArgs e)
            {
                Messages.ShowInfo(HostShip.PilotName + ": Initiative is set to 7");

                HostShip.AddPilotSkillModifier(HostUpgrade as IModifyPilotSkill);
                Phases.Events.OnCombatPhaseEnd_NoTriggers += RestorePilotSkill;

                HostShip.Tokens.RemoveToken(typeof(ForceToken), FinishAbility);
            }

            private void FinishAbility()
            {
                Selection.DeselectAllShips();

                SubPhases.DecisionSubPhase.ConfirmDecision();
            }

            private void RestorePilotSkill()
            {
                HostShip.RemovePilotSkillModifier(HostUpgrade as IModifyPilotSkill);

                Messages.ShowInfo(HostShip.PilotName + ": Initiative is restored");
            }
        }
    }
}