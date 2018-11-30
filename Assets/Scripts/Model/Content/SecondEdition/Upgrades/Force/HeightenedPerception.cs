using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HeightenedPerception : GenericUpgrade, IModifyPilotSkill
    {
        public HeightenedPerception() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Heightened Perception",
                UpgradeType.Force,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.HeightenedPerception),
                seImageNumber: 19
            );
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = 7;
        }
    }
}

namespace Abilities.SecondEdition
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

            HostShip.State.AddPilotSkillModifier(HostUpgrade as IModifyPilotSkill);
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
            HostShip.State.RemovePilotSkillModifier(HostUpgrade as IModifyPilotSkill);

            Messages.ShowInfo(HostShip.PilotName + ": Initiative is restored");
        }
    }
}