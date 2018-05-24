using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;
using System.Linq;
using Ship;

namespace MainPhases
{

    public class SystemsPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Name = "Systems Phase";

            Roster.AllShips.First().Value.OnSystemsPhaseActivation += Test;

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

        private void Test(GenericShip ship)
        {
            Messages.ShowInfo("Ability was activated");
            Roster.AllShips.First().Value.OnSystemsPhaseActivation -= Test;
        }

    }

}
