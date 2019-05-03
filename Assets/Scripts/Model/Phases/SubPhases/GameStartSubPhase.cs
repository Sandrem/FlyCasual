using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SubPhases
{

    public class GameStartSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Game start";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Phases.Events.CallInitialiveSelection(delegate { Phases.FinishSubPhase(typeof(GameStartSubPhase)); });
        }

        public override void Next()
        {
            GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartObstaclesPlacementpPhase);
            (subphase as NotificationSubPhase).TextToShow = (DebugManager.NoObstaclesSetup) ? "Setup" : "Obstacles";
            subphase.Start();
        }

        private void StartObstaclesPlacementpPhase()
        {
            if (!DebugManager.NoObstaclesSetup)
            {
                Phases.CurrentSubPhase = new ObstaclesPlacementSubPhase();
            }
            else
            {
                Phases.CurrentSubPhase = new SetupStartSubPhase();
            }
            
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Prepare();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            return false;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip targetShip, int mouseKeyIsPressed)
        {
            return false;
        }

    }

}
