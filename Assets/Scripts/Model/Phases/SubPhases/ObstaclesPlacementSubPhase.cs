using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BoardTools;

namespace SubPhases
{

    public class ObstaclesPlacementSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Obstacles Placement";
            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Board.ToggleObstaclesHolder(true);
            UI.ShowNextButton();
        }

        public override void Next()
        {
            GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew("Notification", typeof(NotificationSubPhase), StartSetupPhase);
            (subphase as NotificationSubPhase).TextToShow = "Setup";
            subphase.Start();
        }

        private void StartSetupPhase()
        {
            Phases.CurrentSubPhase = new SetupStartSubPhase();
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

        public override void NextButton()
        {
            Board.ToggleObstaclesHolder(false);
            Next();
        }

    }

}
