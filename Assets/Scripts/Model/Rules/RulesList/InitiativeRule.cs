using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using Players;
using GameModes;
using GameCommands;

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
            Phases.Events.OnGameStart += DetermineOwnerOfDecision;
            Phases.Events.OnInitiativeSelection += DeterminePlayerWithInitiative;
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
            GameInitializer.SetState(typeof(SyncPlayerWithInitiativeCommand));

            int costP1 = Roster.GetPlayer(PlayerNo.Player1).SquadCost;
            int costP2 = Roster.GetPlayer(PlayerNo.Player2).SquadCost;

            if (costP1 < costP2)
            {
                GameMode.CurrentGameMode.GiveInitiativeToPlayer(1);
            }
            else if (costP1 > costP2)
            {
                GameMode.CurrentGameMode.GiveInitiativeToPlayer(2);
            }
            else
            {
                int randomPlayer = UnityEngine.Random.Range(1, 3);
                GameMode.CurrentGameMode.GiveInitiativeToPlayer(randomPlayer);
            }
        }

        public static GameCommand GenerateInitiativeDecisionOwnerCommand(int randomPlayer)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("player", Tools.IntToPlayer(randomPlayer).ToString());

            return GameController.GenerateGameCommand(
                GameCommandTypes.SyncPlayerWithInitiative,
                null,
                parameters.ToString()
            );
        }

        public static void DeterminePlayerWithInitiative()
        {
            Phases.CurrentSubPhase.RequiredPlayer = Phases.PlayerWithInitiative;
            Triggers.RegisterTrigger(new Trigger() {
                Name = "Initiative decision",
                TriggerOwner = Phases.PlayerWithInitiative,
                TriggerType = TriggerTypes.OnInitiativeSelection,
                EventHandler = ShowDecision
            });
        }

        private static void ShowDecision(object sender, EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Initiative",
                typeof(SubPhases.InitialiveDecisionSubPhase),
                Triggers.FinishTrigger
            );
        }
    } 
}

namespace SubPhases
{

    public class InitialiveDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(Action callBack)
        {
            AddDecision("Me", StayWithInitiative);
            AddDecision("Opponent", GiveInitiative);

            DefaultDecisionName = "Opponent";

            DescriptionShort = "Initiative";
            string playerName = (Roster.GetPlayer(2) is HumanPlayer) ? "Player " + Tools.PlayerToInt(Phases.PlayerWithInitiative) : Roster.GetPlayer(Phases.PlayerWithInitiative).NickName;
            DescriptionLong = playerName + ", which player should have initiative?";

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
            Messages.ShowInfo(Roster.GetPlayer(Phases.PlayerWithInitiative).NickName + " has Initiative");
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}