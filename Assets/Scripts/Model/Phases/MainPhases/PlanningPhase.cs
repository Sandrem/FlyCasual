using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

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

            GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartActivationPhase);
            (subphase as NotificationSubPhase).TextToShow = "Activation";
            subphase.Start();
        }

        private void StartActivationPhase()
        {
            Phases.CurrentPhase = new ActivationPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}