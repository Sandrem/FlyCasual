﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;
using Movement;
using Obstacles;

namespace SubPhases
{

    public class MovementExecutionSubPhase : GenericSubPhase
    {

        public override void Start()
        {
            base.Start();

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

            Selection.ThisShip.ObstaclesHit = new List<GenericObstacle>();
            Selection.ThisShip.MinesHit = new List<GameObject>();

            CheckAssignedManeuver();
        }

        private void CheckAssignedManeuver()
        {
            if (Selection.ThisShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex && Selection.ThisShip.Tokens.HasToken(typeof(Tokens.StressToken)))
            {
                if (!Selection.ThisShip.CanPerformRedManeuversWhileStressed)
                {
                    Messages.ShowErrorToHuman("Red maneuver while stresses: Maneuver is changed to white straight 2");
                    Selection.ThisShip.SetAssignedManeuver(new StraightMovement(2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal));
                }
            }

            PerformAssignedManeuver();
        }

        private void PerformAssignedManeuver()
        {
            Selection.ThisShip.AssignedManeuver.Perform();
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

    }

}
