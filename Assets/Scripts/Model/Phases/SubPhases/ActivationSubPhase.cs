using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using GameModes;
using GameCommands;
using Remote;

namespace SubPhases
{

    public class ActivationSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.ActivateAndMove }; } }

        private bool IsLocked;

        public override void Start()
        {
            base.Start();

            Name = "Activation SubPhase";
        }

        public override void Prepare()
        {
            RequiredInitiative = PILOTSKILL_MIN - 1;
        }

        public override void Initialize()
        {
            Next();
        }

        public override void Next()
        {
            bool success = GetNextActivation(RequiredInitiative);
            if (!success)
            {
                int nextPilotSkill = GetNextPilotSkill(RequiredInitiative);
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
                Roster.HighlightShipsFiltered(FilterShipsToExecuteManeuver);

                IsLocked = false;
                IsReadyForCommands = true;
                Roster.GetPlayer(RequiredPlayer).PerformManeuver();
            }

        }

        private bool GetNextActivation(int pilotSkill)
        {

            bool result = false;

            var pilotSkillResults =
                from n in Roster.AllShips
                where n.Value.State.Initiative == pilotSkill
                where n.Value.IsManeuverPerformed == false
                select n;

            if (pilotSkillResults.Count() > 0)
            {
                RequiredInitiative = pilotSkill;

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
                where n.Value.IsManeuverPerformed == false
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
            if (Phases.Events.HasOnActivationPhaseEnd)
            {
                GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartActivationEndSubPhase);
                (subphase as NotificationSubPhase).TextToShow = "End of Activation ";
                subphase.Start();
            }
            else
            {
                StartActivationEndSubPhase();
            }
        }

        private void StartActivationEndSubPhase()
        {
            Phases.CurrentSubPhase = new ActivationEndSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = true;

            if (IsLocked)
            {
                result = false;
            }
            else if (ship.Owner.PlayerNo != RequiredPlayer)
            {
                Messages.ShowErrorToHuman("This ship cannot be selected, the ship must be owned by " + RequiredPlayer);
                result = false;
            }
            else if (ship.State.Initiative != RequiredInitiative)
            {
                Messages.ShowErrorToHuman("This ship cannot be selected, the ship must have Initiative " + RequiredInitiative);
                result = false;
            }
            else if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(Players.HumanPlayer))
            {
                Messages.ShowErrorToHuman("Ships cannot be selected: it is not your turn");
                result = false;
            }

            return result;
        }

        // OUTDATED
        public override int CountActiveButtons(GenericShip ship)
        {
            int result = 0;
            if (!Selection.ThisShip.IsManeuverPerformed)
            {
                GameObject.Find("UI").transform.Find("ContextMenuPanel").Find("MovePerformButton").gameObject.SetActive(true);
                result++;
            }
            else
            {
                Messages.ShowErrorToHuman("This ship has already executed their maneuver");
            };
            return result;
        }

        private bool FilterShipsToExecuteManeuver(GenericShip ship)
        {
            return ship.State.Initiative == RequiredInitiative
                && !ship.IsManeuverPerformed
                && ship.Owner.PlayerNo == RequiredPlayer
                && !(ship is GenericRemote);
        }

        public override void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed)
        {
            if (!ship.IsManeuverPerformed && !IsLocked)
            {
                ship.IsManeuverPerformed = true;
                IsLocked = true;

                GameCommand command = ShipMovementScript.GenerateActivateAndMoveCommand(Selection.ThisShip.ShipId);
                GameMode.CurrentGameMode.ExecuteCommand(command);
            }
            else
            {
                Messages.ShowErrorToHuman("This ship has already executed their maneuver");
            };
        }

    }

}
