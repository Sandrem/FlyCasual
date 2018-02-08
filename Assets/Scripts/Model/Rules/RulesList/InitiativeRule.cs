using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using Players;
using GameModes;

namespace RulesList
{
    public class InitiativeRule
    {
        public InitiativeRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Phases.OnGameStart += DetermineOwnerOfDecision;
            Phases.OnSetupPhaseStart += DeterminePlayerWithInitiative;
        }

        public static void DetermineOwnerOfDecision()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Initiative decision owner",
                TriggerOwner = Phases.PlayerWithInitiative,
                TriggerType = TriggerTypes.OnGameStart,
                EventHandler = DetermineOwnerOfDecisionTrigger,
                Skippable = true
            });
        }

        private static void DetermineOwnerOfDecisionTrigger(object sender, System.EventArgs e)
        {
            int costP1 = Roster.GetPlayer(PlayerNo.Player1).SquadCost;
            int costP2 = Roster.GetPlayer(PlayerNo.Player2).SquadCost;

            if (costP1 < costP2)
            {
                Phases.PlayerWithInitiative = PlayerNo.Player1;
                Triggers.FinishTrigger();
            }
            else if (costP1 > costP2)
            {
                Phases.PlayerWithInitiative = PlayerNo.Player2;
                Triggers.FinishTrigger();
            }
            else
            {
                GameMode.CurrentGameMode.GiveInitiativeToRandomPlayer();
            }
        }

        public static void DeterminePlayerWithInitiative()
        {
            Phases.CurrentSubPhase.RequiredPlayer = Phases.PlayerWithInitiative;
            Triggers.RegisterTrigger(new Trigger() {
                Name = "Initiative decision",
                TriggerOwner = Phases.PlayerWithInitiative,
                TriggerType = TriggerTypes.OnSetupPhaseStart,
                EventHandler = ShowDecision
            });
        }

        private static void ShowDecision(object sender, EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Initiative",
                typeof(SubPhases.InitialiveDecisionSubPhase),
                delegate() { Triggers.FinishTrigger();
            });
        }
    } 
}

namespace SubPhases
{

    public class InitialiveDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Player " + Tools.PlayerToInt(Phases.PlayerWithInitiative) + ", which player should have initiative?";

            AddDecision("Me", StayWithInitiative);
            AddDecision("Opponent", GiveInitiative);

            DefaultDecisionName = "Opponent";

            callBack();
        }

        private void GiveInitiative(object sender, EventArgs e)
        {
            Phases.PlayerWithInitiative = Roster.AnotherPlayer(Phases.PlayerWithInitiative);
            InformConfirmDecision();
        }

        private void StayWithInitiative(object sender, EventArgs e)
        {
            InformConfirmDecision();
        }

        private void InformConfirmDecision()
        {
            Messages.ShowInfo("Player " + Tools.PlayerToInt(Phases.PlayerWithInitiative) + " has Initiative");
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}