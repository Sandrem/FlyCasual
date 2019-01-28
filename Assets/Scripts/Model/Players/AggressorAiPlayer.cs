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

            AssignManeuverRecursive();
        }

        private void AssignManeuverRecursive()
        {
            GenericShip shipWithoutManeuver = Ships.Values.FirstOrDefault(n => n.AssignedManeuver == null);

            if (shipWithoutManeuver != null)
            {
                Selection.ChangeActiveShip(shipWithoutManeuver);
                AI.Aggressor.NavigationSubSystem.CalculateNavigation(Selection.ThisShip, FinishAssignManeuver);
            }
            else
            {
                GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateNextButtonCommand());
            }
        }

        private void FinishAssignManeuver()
        {
            ShipMovementScript.SendAssignManeuverCommand(Selection.ThisShip.ShipId, AI.Aggressor.NavigationSubSystem.BestManeuver);

            AssignManeuverRecursive();
        }

        protected override GenericShip SelectTargetForAttack()
        {
            if (DebugManager.DebugNoCombat) return null;

            return AI.Aggressor.TargetingSubSystem.SelectTargetAndWeapon(Selection.ThisShip);
        }
    }
}
