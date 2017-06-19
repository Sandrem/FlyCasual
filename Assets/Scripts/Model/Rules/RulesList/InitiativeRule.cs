using System.Collections;
using UnityEngine;
using System;
using Players;

namespace RulesList
{
    public class InitiativeRule
    {
        private GameManagerScript Game;

        public InitiativeRule(GameManagerScript game)
        {
            Game = game;
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Phases.OnSetupPhaseStart += DeterminePlayerWithInitiative;
        }

        public static void DeterminePlayerWithInitiative()
        {
            int costP1 = Roster.GetPlayer(PlayerNo.Player1).SquadCost;
            int costP2 = Roster.GetPlayer(PlayerNo.Player2).SquadCost;

            if (costP1 < costP2)
            {
                Phases.PlayerWithInitiative = PlayerNo.Player1;
            }
            else if (costP1 > costP2)
            {
                Phases.PlayerWithInitiative = PlayerNo.Player2;
            }
            else
            {
                int randomPlayer = UnityEngine.Random.Range(1, 3);
                Phases.PlayerWithInitiative = Tools.IntToPlayer(randomPlayer);
            }

            Phases.CurrentSubPhase.RequiredPlayer = Phases.PlayerWithInitiative;
            Phases.StartTemporarySubPhase("Initiative", typeof(SubPhases.InitialiveDecisionSubPhase));
        }
    } 
}

namespace SubPhases
{

    public class InitialiveDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Player " + Tools.PlayerToInt(Phases.PlayerWithInitiative) + ", what player will have initiative?";

            decisions.Add("I", StayWithInitiative);
            decisions.Add("Opponent", GiveInitiative);

            defaultDecision = "Opponent";
        }

        private void GiveInitiative(object sender, EventArgs e)
        {
            Phases.PlayerWithInitiative = Roster.AnotherPlayer(Phases.PlayerWithInitiative);
            ConfirmDecision();
        }

        private void StayWithInitiative(object sender, EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Messages.ShowInfo("Player " + Tools.PlayerToInt(Phases.PlayerWithInitiative) + " has Initiative");
            Phases.Next();
        }

    }

}