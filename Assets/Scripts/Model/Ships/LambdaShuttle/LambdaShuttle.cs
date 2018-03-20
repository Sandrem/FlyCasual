using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class LambdaShuttle : GenericShip
        {

            public LambdaShuttle() : base()
            {
                Type = "Lambda-class Shuttle";
                IconicPilots.Add(Faction.Imperial, typeof(CaptainYorr));
                ShipBaseSize = BaseSize.Large;

                ManeuversImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/c/c3/MI_LAMBDA-SHUTTLE.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 5;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Cannon);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.LambdaShuttleTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Default";

                SoundShotsPath = "Slave1-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 3; i++)
                {
                    SoundFlyPaths.Add("Slave1-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("0.S.S", ManeuverColor.Red);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.L.T", ManeuverColor.Red);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.Red);
                Maneuvers.Add("3.L.B", ManeuverColor.Red);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.Red);
            }

        }
    }
}
