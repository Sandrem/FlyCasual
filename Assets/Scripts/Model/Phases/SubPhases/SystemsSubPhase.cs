using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using GameModes;
using GameCommands;

namespace SubPhases
{

    public class SystemsSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.SystemActivation, GameCommandTypes.PressSkip }; } }

        public override void Start()
        {
            base.Start();

            Name = "Systems SubPhase";
        }

        public override void Prepare()
        {
            RequiredPilotSkill = PILOTSKILL_MIN - 1;
        }

        public override void Initialize()
        {
            Next();
        }

        public override void Next()
        {
            UI.HideSkipButton();

            bool success = GetNextActivation(RequiredPilotSkill);
            if (!success)
            {
                int nextPilotSkill = GetNextPilotSkill(RequiredPilotSkill);
                if (nextPilotSkill != int.MinValue)
                {
                    success = GetNextActivation(nextPilotSkill);
                }
                else
                {
                    FinishPhase();
                }
            }

            if (success)
            {
                UpdateHelpInfo();
                Roster.HighlightShipsFiltered(FilterShipsWithActiveDevice);

                IsReadyForCommands = true;
                Roster.GetPlayer(RequiredPlayer).PerformSystemsActivation();
            }
        }

        private bool GetNextActivation(int pilotSkill)
        {

            bool result = false;

            var pilotSkillResults =
                from n in Roster.AllShips
                where n.Value.State.Initiative == pilotSkill
                where n.Value.IsSystemsAbilityCanBeActivated == true
                select n;

            if (pilotSkillResults.Count() > 0)
            {
                RequiredPilotSkill = pilotSkill;

                var playerNoResults =
                    from n in pilotSkillResults
                    where n.Value.Owner.PlayerNo == Phases.PlayerWithInitiative
                    select n;

                if (playerNoResults.Count() > 0)
                {
                    RequiredPlayer = Phases.PlayerWithInitiative;
                }
                else
                {
                    RequiredPlayer = Roster.AnotherPlayer(Phases.PlayerWithInitiative);
                }

                result = true;
            }

            return result;
        }

        private int GetNextPilotSkill(int pilotSkillMin)
        {
            int result = int.MinValue;

            var ascPilotSkills =
                from n in Roster.AllShips
                where n.Value.State.Initiative > pilotSkillMin
                where n.Value.IsSystemsAbilityCanBeActivated == true
                orderby n.Value.State.Initiative
                select n;

            if (ascPilotSkills.Count() > 0)
            {
                result = ascPilotSkills.First().Value.State.Initiative;
            }

            return result;
        }

        public override void FinishPhase()
        {
            Phases.CurrentPhase.NextPhase();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;

            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.State.Initiative == RequiredPilotSkill) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                result = true;
            }
            else
            {
                Messages.ShowErrorToHuman("Ship cannot be selected:\n Need " + RequiredPlayer + " and pilot skill " + RequiredPilotSkill);
            }
            return result;
        }

        private bool FilterShipsWithActiveDevice(GenericShip ship)
        {
            return ship.State.Initiative == RequiredPilotSkill && ship.IsSystemsAbilityCanBeActivated && ship.Owner.PlayerNo == RequiredPlayer;
        }

        public override void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed)
        {
            if (ship.IsSystemsAbilityCanBeActivated)
            {
                GameMode.CurrentGameMode.ExecuteCommand(GenerateSystemActivationCommand(Selection.ThisShip.ShipId));
            }
            else
            {
                Messages.ShowErrorToHuman("This ship doesn't have any abilities to activate");
            };
        }

        public GameCommand GenerateSystemActivationCommand(int shipId)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("id", shipId.ToString());
            return GameController.GenerateGameCommand(
                GameCommandTypes.SystemActivation,
                Phases.CurrentSubPhase.GetType(),
                parameters.ToString()
            );
        }

        public static void DoSystemActivation(int shipId)
        {
            GenericShip ship = Roster.GetShipById("ShipId:" + shipId);
            ship.CallOnSystemsPhaseActivation((Phases.CurrentSubPhase as SystemsSubPhase).Next);
        }

        public override void SkipButton()
        {
            foreach (GenericShip ship in Roster.GetPlayer(RequiredPlayer).Ships.Values)
            {
                if (ship.State.Initiative == RequiredPilotSkill) ship.IsSystemsAbilityInactive = true;
            }

            Next();
        }

    }

}
