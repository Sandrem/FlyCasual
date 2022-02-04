using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;
using Ship;
using System.Linq;

namespace MainPhases
{

    public class PlanningPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Name = "Planning Phase";

            Phases.CurrentSubPhase = new RoundStartSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override void NextPhase()
        {
            Selection.DeselectAllShips();

            // Check if any ship has system ability (bombs etc)
            bool anyShipHasSystemsAbility = Roster.AllUnits.Values.Any(n => n.IsSystemsAbilityCanBeActivated);

            if (anyShipHasSystemsAbility)
            {
                GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), FinishPlanningPhase);
                (subphase as NotificationSubPhase).TextToShow = "Systems";
                subphase.Start();
            }
            else
            {
                FinishPlanningPhase();
            }
        }

        private void FinishPlanningPhase()
        {
            int randomPlayer = UnityEngine.Random.Range(1, 3);
            if (randomPlayer != Tools.PlayerToInt(Phases.PlayerWithInitiative)) Messages.ShowInfo("First Player is changed");
            Phases.PlayerWithInitiative = Tools.IntToPlayer(randomPlayer);

            StartSystemsPhase();
        }

        private void StartSystemsPhase()
        {
            Phases.CurrentPhase = new SystemsPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}