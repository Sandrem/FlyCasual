using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using GameModes;

namespace SubPhases
{

    public class ActivationSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            base.Start();

            Name = "Activation SubPhase";
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
                Roster.HighlightShipsFiltered(FilterShipsToPerformAttack);
                Roster.GetPlayer(RequiredPlayer).PerformManeuver();
            }

        }

        private bool GetNextActivation(int pilotSkill)
        {

            bool result = false;

            var pilotSkillResults =
                from n in Roster.AllShips
                where n.Value.PilotSkill == pilotSkill
                where n.Value.IsManeuverPerformed == false
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
            if (Phases.HasOnActivationPhaseEnd)
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
            bool result = false;

            if ((ship.Owner.PlayerNo == RequiredPlayer) && (ship.PilotSkill == RequiredPilotSkill) && (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer)))
            {
                result = true;
            }
            else
            {
                Messages.ShowErrorToHuman("Ship cannot be selected:\n Need " + RequiredPlayer + " and pilot skill " + RequiredPilotSkill);
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
                Messages.ShowErrorToHuman("This ship has already executed his maneuver");
            };
            return result;
        }

        private bool FilterShipsToPerformAttack(GenericShip ship)
        {
            return ship.PilotSkill == RequiredPilotSkill && !ship.IsManeuverPerformed && ship.Owner.PlayerNo == RequiredPlayer;
        }

        public override void DoSelectThisShip(GenericShip ship, int mouseKeyIsPressed)
        {
            if (!ship.IsManeuverPerformed)
            {
                GameMode.CurrentGameMode.PerformStoredManeuver(Selection.ThisShip.ShipId);
            }
            else
            {
                Messages.ShowErrorToHuman("This ship has already executed his maneuver");
            };
        }

    }

}
