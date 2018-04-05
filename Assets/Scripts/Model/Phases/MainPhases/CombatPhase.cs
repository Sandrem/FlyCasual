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

            if (Phases.HasOnCombatPhaseStartEvents)
            {
                GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartCombatStartSubPhase);
                (subphase as NotificationSubPhase).TextToShow = "Start of Combat";
                subphase.Start();
            }
            else
            {
                StartCombatStartSubPhase();
            }
        }

        private void StartCombatStartSubPhase()
        {
            Phases.CurrentSubPhase = new CombatStartSubPhase();
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override void NextPhase()
        {
            Selection.DeselectAllShips();

            if (Phases.HasOnEndPhaseStartEvents)
            {
                GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartEndPhase);
                (subphase as NotificationSubPhase).TextToShow = "End";
                subphase.Start();
            }
            else
            {
                StartEndPhase();
            }
        }

        private void StartEndPhase()
        {
            Phases.CurrentPhase = new EndPhase();
            Phases.CurrentPhase.StartPhase();
        }

    }

}
