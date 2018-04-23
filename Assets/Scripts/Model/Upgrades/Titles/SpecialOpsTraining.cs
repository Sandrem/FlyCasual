using Ship;
using Ship.TIESF;
using Upgrade;
using SubPhases;
using Abilities;

namespace UpgradesList
{
    public class SpecialOpsTraining : GenericUpgrade
    {
        public SpecialOpsTraining() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Special Ops Training";            
            Cost = 0;            

            UpgradeAbilities.Add(new SpecialOpsTrainingAbility());            
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIESF;
        }
        
    }
}

//Special ops
namespace Abilities
{
    public class SpecialOpsTrainingAbility : GenericAbility
    {        
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
        }

        private void CheckConditions()
        {
            if (IsAbilityUsed) return;
            if (Combat.ChosenWeapon.GetType() != typeof(PrimaryWeaponClass)) return;                        
            if (!Combat.ShotInfo.InPrimaryArc) return;            
            
            RegisterAbilityTrigger(TriggerTypes.OnShotStart, StartQuestionSubphase);            
        }

        private class SpecialOpsTrainingDecisionSubPhase : DecisionSubPhase { }

        private void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            if (!IsAbilityUsed)
            {
                SpecialOpsTrainingDecisionSubPhase selectSpecialOpsTrainingSubPhase = (SpecialOpsTrainingDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(SpecialOpsTrainingDecisionSubPhase),
                    Triggers.FinishTrigger
                );

                selectSpecialOpsTrainingSubPhase.InfoText = "Use " + Name + "?";
                selectSpecialOpsTrainingSubPhase.AddDecision("Roll 1 extra die from primary fire arc", RegisterRollExtraDice);
                selectSpecialOpsTrainingSubPhase.AddTooltip("Roll 1 extra die from primary fire arc", HostShip.ImageUrl);
                selectSpecialOpsTrainingSubPhase.AddDecision("Get a second attack from rear arc", RegisterExtraAttack);
                selectSpecialOpsTrainingSubPhase.AddTooltip("Get a second attack from rear arc", HostShip.ImageUrl);
                selectSpecialOpsTrainingSubPhase.DefaultDecisionName = GetDefaultDecision();
                selectSpecialOpsTrainingSubPhase.ShowSkipButton = true;
                selectSpecialOpsTrainingSubPhase.Start();
            } else
            {
                Messages.ShowErrorToHuman("Special Ops Training ability has already been used");
                Triggers.FinishTrigger();
            }
        }

        private string GetDefaultDecision()
        {
            string result = "Roll 1 extra die from primary fire arc";
            //TODO : Check if we have targets in both arcs
            return result;
        }

        private void RegisterRollExtraDice(object sender, System.EventArgs e)
        {                        
            IsAbilityUsed = true;
            HostShip.OnAttackStartAsAttacker += ClearAbility;

            HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            DecisionSubPhase.ConfirmDecision();                            
        }

        private void RollExtraDice(ref int count)
        {
            count++;            
            Messages.ShowInfo("TieSF rolls extra die from primary arc");
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }

        private void RegisterExtraAttack(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostShip.OnCombatCheckExtraAttack += RegisterSpecialOpsExtraAttack;
            DecisionSubPhase.ConfirmDecision();            
        }

        private void RegisterSpecialOpsExtraAttack(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSpecialOpsExtraAttack;
            this.ToggleFrontArc(false);            
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoSpecialOpsExtraAttack);
        }

        private void DoSpecialOpsExtraAttack(object sender, System.EventArgs e)
        {

            Combat.StartAdditionalAttack(
                HostShip,
                delegate {
                    Selection.ThisShip.IsAttackPerformed = true;
                    ToggleFrontArc(true);
                    HostShip.OnAttackStartAsAttacker += ClearAbility;
                    Triggers.FinishTrigger();
                },
                null,
                HostUpgrade.Name,
                "You may perfrom an additional attack from your auxiliary firing arc.",
                HostUpgrade.ImageUrl
            );                
        }

        //Arc toggle
        private void ToggleFrontArc(bool isActive)
        {
            HostShip.ArcInfo.GetPrimaryArc().ShotPermissions.CanShootPrimaryWeapon = isActive;
        }

        // IsAbilityUsed to avoid asking question during second attack
        private void ClearAbility()
        {
            HostShip.OnAttackStartAsAttacker -= ClearAbility;

            IsAbilityUsed = false;
        }

    }
}