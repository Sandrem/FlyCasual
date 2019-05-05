using Ship;
using Upgrade;
using ActionsList;
using SubPhases;
using Actions;
using Tokens;
using System;

namespace UpgradesList.SecondEdition
{
    public class C3P0Rebel : GenericUpgrade
    {
        public C3P0Rebel() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "C-3PO",
                UpgradeType.Crew,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                addAction: new ActionInfo(typeof(CalculateAction)),
                abilityType: typeof(Abilities.SecondEdition.C3P0RebelCrewAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/Card_Upgrade_80.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class C3P0RebelCrewAbility : GenericAbility
    {
        //Before rolling defense dice, you may spend 1 calculate token to guess aloud a number 1 or higher. 
        //If you do and you roll exactly that many evade results, add 1 evade result.
        //After you perform the calculate action, gain 1 calculate token.
        //Adds calculate action
        bool addedAbility = false;
        int numberGuessed = -1;

        public override void ActivateAbilityForSquadBuilder()
        {
            if (HostShip.PilotAbilities.Find(ability => ability is AdvancedDroidBrain) == null)
            {
                HostShip.PilotAbilities.Add(new AdvancedDroidBrain());
                addedAbility = true;
            }
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            if (addedAbility)
            {
                HostShip.PilotAbilities.RemoveAll(ability => ability is AdvancedDroidBrain);
            }
        }

        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsDefender += C3P0RebelEffect;
        }
                
        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsDefender -= C3P0RebelEffect;
        }

        private void C3P0RebelEffect()
        {
            if(HostShip.Tokens.HasToken(typeof(CalculateToken))){
                RegisterAbilityTrigger(TriggerTypes.OnDefenseStart, ShowDecision);
            } 
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                UseAbility,
                infoText: HostShip.PilotInfo.PilotName + ": Spend a Calculate Token to guess a number 1 or higher. "
                    + "If you do and you roll exactly that many evade results, add 1 evade result. " 
            );
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            SpendCalculateToken();

            var selectionSubPhase = (EvadeCountSelectionSubPhase)Phases.StartTemporarySubPhaseNew(
                "C3PO - Guess number of evade results",
                typeof(EvadeCountSelectionSubPhase),
                Triggers.FinishTrigger
            );

            selectionSubPhase.InfoText = String.Format("You may choose a number greater than 1. If you roll exactly that many evade results, add 1 evade result.");

            for (var i = 1; i <= 6; i++) //TODO: likely needs to be more than 6, add a way to increase numbers
            {
                int option = i;
                selectionSubPhase.AddDecision(option.ToString(),
                    delegate 
                    { 
                        this.numberGuessed = option;
                        SubPhases.DecisionSubPhase.ConfirmDecision(); 
                    }
                );
            }

            selectionSubPhase.DefaultDecisionName = "1";
            selectionSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            selectionSubPhase.Start();
        }

        private void SpendCalculateToken()
        {
            HostShip.Tokens.SpendToken(typeof(Tokens.CalculateToken), delegate
            {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                HostShip.OnImmediatelyAfterRolling += CheckGuess; 
            });
        }
            
        private class EvadeCountSelectionSubPhase : DecisionSubPhase { }

        private void CheckGuess(DiceRoll diceroll)
        {
            if(numberGuessed == diceroll.Successes)
            {
                AddEvadeDie(diceroll);
            } 
            HostShip.OnImmediatelyAfterRolling -= CheckGuess; 
        }
        private void AddEvadeDie(DiceRoll diceroll)
        {
            Messages.ShowInfo("C-3PO: added evade for correct guess.");
            diceroll.AddDice(DieSide.Success).ShowWithoutRoll();
            diceroll.OrganizeDicePositions();
        }
    }
}