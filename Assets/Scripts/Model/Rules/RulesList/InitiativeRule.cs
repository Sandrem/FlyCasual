using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using Players;

namespace RulesList
{
    public class InitiativeRule
    {
        public InitiativeRule(GameManagerScript game)
        {
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
            Triggers.RegisterTrigger(new Trigger() { Name = "Initiative decision", TriggerOwner = Phases.PlayerWithInitiative, triggerType = TriggerTypes.OnSetupPhaseStart, eventHandler = ShowDecision });
        }

        private static void ShowDecision(object sender, EventArgs e)
        {
            Phases.StartTemporarySubPhase("Initiative", typeof(SubPhases.InitialiveDecisionSubPhase), delegate() { Triggers.FinishTrigger(); });
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

            AddDecision("I", StayWithInitiative);
            AddDecision("Opponent", GiveInitiative);

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
            Phases.FinishSubPhase(this.GetType());
            callBack();
        }

    }

}