﻿using Ship;
using Upgrade;
using ActionsList;
using SubPhases;
using Actions;
using Tokens;
using System;
using System.Linq;

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
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                UseAbility,
                descriptionLong: "Do you want to spend a Calculate Token to guess a number of evade results? (If you roll exactly that many Evade results, add 1 Evade result)",
                imageHolder: HostUpgrade
            );
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.OnImmediatelyAfterRolling += CheckGuess;

            HostShip.Tokens.SpendToken(typeof(Tokens.CalculateToken), ShowCustomDecision);
        }

        private void ShowCustomDecision()
        {
            var selectionSubPhase = (EvadeCountSelectionSubPhase)Phases.StartTemporarySubPhaseNew(
                "C3PO - Guess number of evade results",
                typeof(EvadeCountSelectionSubPhase),
                Triggers.FinishTrigger
            );

            selectionSubPhase.DescriptionShort = "C-3PO";
            selectionSubPhase.DescriptionLong = String.Format("You may choose a number 1 or higher. If you roll exactly that many evade results, add 1 evade result.");
            selectionSubPhase.ImageSource = HostUpgrade;

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
            
        private class EvadeCountSelectionSubPhase : DecisionSubPhase { }

        private void CheckGuess(DiceRoll diceroll)
        {
            //compare number guessed to successes rolled (not added)
            if (numberGuessed == diceroll.DiceList.Count(n => n.IsSuccess && !n.IsAddedResult))
            {
                AddEvadeDie(diceroll);
            } 
            HostShip.OnImmediatelyAfterRolling -= CheckGuess; 
        }

        private void AddEvadeDie(DiceRoll diceroll)
        {
            Messages.ShowInfo("C-3PO: added evade for correct guess");
            diceroll.AddDice(DieSide.Success).ShowWithoutRoll();
            diceroll.OrganizeDicePositions();
        }
    }
}