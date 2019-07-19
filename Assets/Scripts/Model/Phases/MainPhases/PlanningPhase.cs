﻿using System.Collections;
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
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                ship.CallOnSystemsPhaseActivationGenerateListeners();
            }
            bool anyShipHasSystemsAbility = Roster.AllShips.Values.Any(n => n.IsSystemsAbilityCanBeActivated);

            if (anyShipHasSystemsAbility)
            {
                GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartSystemsPhase);
                (subphase as NotificationSubPhase).TextToShow = "Systems";
                subphase.Start();
            }
            else
            {
                StartSystemsPhase();
            }
        }

        private void StartSystemsPhase()
        {
            Phases.CurrentPhase = new SystemsPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}