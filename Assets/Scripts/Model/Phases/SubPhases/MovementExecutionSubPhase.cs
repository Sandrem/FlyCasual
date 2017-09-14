using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class MovementExecutionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            Name = "Movement";
            RequiredPilotSkill = PreviousSubPhase.RequiredPilotSkill;
            RequiredPlayer = PreviousSubPhase.RequiredPlayer;
            UpdateHelpInfo();

            Initialize();
        }

        public override void Initialize()
        {
            UI.HideContextMenu();

            Selection.ThisShip.IsManeuverPerformed = true;
            Roster.AllShipsHighlightOff();

            Selection.ThisShip.ObstaclesHit = new List<Collider>();
            Selection.ThisShip.MinesHit = new List<GameObject>();

            Selection.ThisShip.CallManeuverIsReadyToBeRevealed();

            Selection.ThisShip.CallManeuverIsRevealed(Selection.ThisShip.AssignedManeuver.Perform);
        }

        public override void Next()
        {
            GenericSubPhase actionSubPhase = new ActionSubPhase();
            actionSubPhase.PreviousSubPhase = Phases.CurrentSubPhase;
            Phases.CurrentSubPhase = actionSubPhase;
            Phases.CurrentSubPhase.Start();
            Phases.CurrentSubPhase.Initialize();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;
            return result;
        }

    }

}
