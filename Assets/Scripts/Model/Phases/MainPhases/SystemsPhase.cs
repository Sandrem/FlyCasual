using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace MainPhases
{

    public class SystemsPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Name = "Systems Phase";

            Phases.CurrentSubPhase = new SystemsSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override void NextPhase()
        {
            Selection.DeselectAllShips();

            GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Activation", typeof(NotificationSubPhase), StartActivationPhase);
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
