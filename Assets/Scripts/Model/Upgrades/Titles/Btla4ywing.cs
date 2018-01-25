using Ship;
using Ship.YWing;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class BTLA4 : GenericUpgrade
    {
        public BTLA4() : base()
        {
            Type = UpgradeType.Title;
            Name = "BTL-A4 Y-wing";            
            Cost = 0;            

            UpgradeAbilities.Add(new BTLA4Ability());            
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YWing;
        }
    }
}

//Special ops
namespace Abilities
{
    public class BTLA4Ability : GenericAbility
    {        
        public override void ActivateAbility() {
            Toggle360Arc(false);
            HostShip.OnShotStartAsAttacker += CheckConditions;
            Phases.OnRoundEnd += ClearAbility;
        }

        public override void DeactivateAbility() {
            Toggle360Arc(true);
            HostShip.OnShotStartAsAttacker -= CheckConditions;
            Phases.OnRoundEnd -= ClearAbility;
        }

        private void Toggle360Arc(bool isActive)
        {
            GenericSecondaryWeapon turret = (GenericSecondaryWeapon)HostShip.UpgradeBar.GetInstalledUpgrades().Find(n => n.Type == UpgradeType.Turret);            
            if (turret != null)
            {
                HostShip.ArcInfo.OutOfArcShotPermissions.CanShootTurret = isActive;
                turret.CanShootOutsideArc = isActive;
            }
        }

        private void CheckConditions()
        {
            if (IsAbilityUsed) return;
            if (Combat.ChosenWeapon.GetType() != typeof(PrimaryWeaponClass)) return;
            if (!Combat.ShotInfo.InPrimaryArc) return;

            IsAbilityUsed = true;
            HostShip.OnCombatCheckExtraAttack += RegisterBTLA4ExtraAttack;
        }

        private void RegisterBTLA4ExtraAttack()
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterBTLA4ExtraAttack;            
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoBTL4AExtraAttack);
        }

        private void DoBTL4AExtraAttack(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotName + " can perform second attack from secondary weapon");

            Combat.StartAdditionalAttack(
                HostShip,
                delegate {
                    Selection.ThisShip.IsAttackPerformed = true;                    
                    Triggers.FinishTrigger();
                },
                IsSecondaryShot
            );
        }

        private bool IsSecondaryShot(GenericShip defender, IShipWeapon weapon)
        {
            bool result = false;
            if (weapon.GetType() != typeof(PrimaryWeaponClass))
            {
                result = true;
            }
            else
            {
                Messages.ShowError("Attack must be performed from secondary weapon");
            }
            return result;
        }

        private void ClearAbility()
        {
            IsAbilityUsed = false;

        }
        
        
        /*
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
                selectSpecialOpsTrainingSubPhase.DefaultDecision = GetDefaultDecision();
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
            HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            DecisionSubPhase.ConfirmDecision();                            
        }

        private void RollExtraDice(ref int count)
        {
            count++;            
            Messages.ShowInfo("TieSF rolls extra die from primary arc");
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }

        

        private void RegisterSpecialOpsExtraAttack()
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSpecialOpsExtraAttack;
            this.ToggleFrontArc(false);            
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoSpecialOpsExtraAttack);
        }

        private void DoSpecialOpsExtraAttack(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotName + " can perform second attack from primary weapon");

            Combat.StartAdditionalAttack(
                HostShip,
                delegate {
                    Selection.ThisShip.IsAttackPerformed = true;
                    ToggleFrontArc(true);
                    Triggers.FinishTrigger();
                }
            );                
        }

        //Arc toggle
        private void ToggleFrontArc(bool isActive)
        {
            HostShip.ArcInfo.GetPrimaryArc().ShotPermissions.CanShootPrimaryWeapon = isActive;
        }        

       
        */
    }
}