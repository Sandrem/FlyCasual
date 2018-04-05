using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubPhases;

namespace MainPhases
{

    public class CombatPhase : GenericPhase
    {

        public override void StartPhase()
        {
            Name = "Combat Phase";

            Phases.CurrentSubPhase = new CombatStartSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override void NextPhase()
        {
            Selection.DeselectAllShips();

            GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartEndPhase);
            (subphase as NotificationSubPhase).TextToShow = "End";
            subphase.Start();
        }

        private void StartEndPhase()
        {
            Phases.CurrentPhase = new EndPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}
