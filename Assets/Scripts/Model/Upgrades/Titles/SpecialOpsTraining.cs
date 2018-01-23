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
            Type = UpgradeType.Title;
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
        public override void ActivateAbility() {
            HostShip.OnShotStartAsAttacker += CheckConditions;
            Phases.OnRoundEnd += ClearAbility;
        }

        public override void DeactivateAbility() {
            HostShip.OnShotStartAsAttacker -= CheckConditions;
            Phases.OnRoundEnd -= ClearAbility;
        }

        private void CheckConditions()
        {
            if (IsAbilityUsed) return;

            if (Combat.ChosenWeapon.GetType() != typeof(PrimaryWeaponClass)) return;

            Board.ShipShotDistanceInformation AttackInfo = new Board.ShipShotDistanceInformation(Combat.Defender, Combat.Attacker);                        
            if (!AttackInfo.InPrimaryArc) return;            


            RegisterAbilityTrigger(TriggerTypes.OnShotStart, StartQuestionSubphase);            
        }

        private class SpecialOpsTrainingDecisionSubPhase : DecisionSubPhase { }
        private void StartQuestionSubphase(object sender, System.EventArgs e)
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
        }

        private string GetDefaultDecision()
        {
            string result = "Roll 1 extra die from primary fire arc";
            //Check if we have targets in both arcs
            return result;
        }

        private void RegisterRollExtraDice(object sender, System.EventArgs e)
        {            
            if (!IsAbilityUsed) {
                IsAbilityUsed = true;
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
                DecisionSubPhase.ConfirmDecision();                
            }
        }

        private void RollExtraDice(ref int count)
        {
            count++;            
            Messages.ShowInfo("TieSF rolls extra die from primary arc");
            HostShip.AfterGotNumberOfAttackDice -= RollExtraDice;
        }

        private void RegisterExtraAttack(object sender, System.EventArgs e)
        {
            if (!IsAbilityUsed)
            {
                IsAbilityUsed = true;
                //HostShip.OnCombatCheckExtraAttack += RegisterIG88BAbility;
                HostShip.OnCombatCheckExtraAttack += RegisterSpecialOpsExtraAttack;
                DecisionSubPhase.ConfirmDecision();
            }
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

        //Spegne un arco
        private void ToggleFrontArc(bool isActive)
        {
            HostShip.ArcInfo.GetPrimaryArc().ShotPermissions.CanShootPrimaryWeapon = isActive;
        }

        //Controlla l'arco
        /*private void CheckAssignStress()
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(HostShip, TargetShip);
            if (shotInfo.InMobileArc && shotInfo.Range >= 1 && shotInfo.Range <= 2)
            {
                Messages.ShowError(HostShip.PilotName + " assigns Stress Token\nto " + TargetShip.PilotName);
                TargetShip.AssignToken(new Tokens.StressToken(), SelectShipSubPhase.FinishSelection);
            }
            else
            {
                if (!shotInfo.InMobileArc) Messages.ShowError("Target is not inside Mobile Arc");
                else if (shotInfo.Distance >= 3) Messages.ShowError("Target is outside range 2");
            }
        }*/

        private void ClearAbility()
        {
            IsAbilityUsed = false;
            
        }

    }
}