using GameModes;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                if (RulesList.IonizationRule.IsIonized(ship)) continue;

                Selection.ChangeActiveShip(ship);
                var maneuvers = ship.GetManeuvers();
                string maneuverString = maneuvers.Keys.ToList()[rnd.Next(maneuvers.Count)];

                ShipMovementScript.SendAssignManeuverCommand(ship.ShipId, maneuverString);
            }
            GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateNextButtonCommand());
        }

    }
}
