using GameModes;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Players
{

    public partial class AggressorAiPlayer : GenericAiPlayer
    {

        public AggressorAiPlayer() : base()
        {
            Name = "Aggressor AI";

            NickName = "Aggressor";
            Title = "Assassin Droid";
            Avatar = "UpgradesList.FirstEdition.IG88D";
        }

        public override void AssignManeuver()
        {
            base.AssignManeuver();

            System.Random rnd = new System.Random();

            foreach (var ship in Ships.Values)
            {
                Selection.ChangeActiveShip(ship);
                AI.Aggressor.NavigationSubSystem.CalculateNavigation(FinishAssignManeuver);
            }

        }

        private void FinishAssignManeuver()
        {
            ShipMovementScript.SendAssignManeuverCommand(Selection.ThisShip.ShipId, AI.Aggressor.NavigationSubSystem.BestManeuver);

            foreach (GenericShip ship in Selection.ThisShip.Owner.Ships.Values)
            {
                if (ship.AssignedManeuver == null) return;
            }

            GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateNextButtonCommand());
        }
    }
}
