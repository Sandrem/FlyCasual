using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class SetupSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Setup SubPhase";
        }

        public override void Prepare()
        {
            RequiredPilotSkill = PILOTSKILL_MIN - 1;
        }

        public override void Initialize()
        {
            Phases.FinishSubPhase(typeof(SetupSubPhase));
        }

        public override void Next()
        {
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
                HighlightShips();
                Roster.GetPlayer(RequiredPlayer).SetupShip();
            }
        }

        private bool GetNextActivation(int pilotSkill)
        {

            bool result = false;

            var pilotSkillResults =
                from n in Roster.AllShips
                where n.Value.PilotSkill == pilotSkill
                where n.Value.IsSetupPerformed == false
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
                where n.Value.PilotSkill > pilotSkillMin
                orderby n.Value.PilotSkill
                select n;

            if (ascPilotSkills.Count() > 0)
            {
                result = ascPilotSkills.First().Value.PilotSkill;
            }

            return result;
        }

        public override void FinishPhase()
        {
            Board.BoardManager.TurnOffStartingZones();
            Phases.NextPhase();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                if (ship.IsSetupPerformed == false)
                {
                    result = true;
                }
                else
                {
                    Messages.ShowErrorToHuman("Ship cannot be selected: Starting position is already set");
                }
            }
            else
            {
                Messages.ShowErrorToHuman("Ship cannot be selected:\n Need " + Phases.CurrentSubPhase.RequiredPlayer + " and pilot skill " + Phases.CurrentSubPhase.RequiredPilotSkill);
            }
            return result;
        }

        private void HighlightShips()
        {
            Roster.AllShipsHighlightOff();
            foreach (var ship in Roster.GetPlayer(RequiredPlayer).Ships)
            {
                if ((ship.Value.PilotSkill == RequiredPilotSkill) && (!ship.Value.IsSetupPerformed))
                {
                    ship.Value.HighlightCanBeSelectedOn();
                    Roster.RosterPanelHighlightOn(ship.Value);
                }
            }
        }

        public void ConfirmShipSetup(int shipId, Vector3 position, Vector3 angles)
        {
            Roster.SetRaycastTargets(true);

            //Temporary
            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Position.inReposition = false;

            Selection.ChangeActiveShip("ShipId:" + shipId);

            Selection.ThisShip.SetPosition(position);
            Selection.ThisShip.SetAngles(angles);
            Selection.ThisShip.IsSetupPerformed = true;

            Selection.DeselectThisShip();
            Board.BoardManager.TurnOffStartingZones();

            Phases.Next();
        }

    }

}
