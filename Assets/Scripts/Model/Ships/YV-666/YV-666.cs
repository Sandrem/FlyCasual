using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace YV666
    {
        public class YV666 : GenericShip
        {

            public YV666() : base()
            {
                Type = "YV-666";
                IconicPilot = "Moralo Eval";
                ShipBaseSize = BaseSize.Large;
                ShipBaseArcsType = Arcs.BaseArcsType.Arc180;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/b/be/MS_YV-666-FREIGHTER.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 6;
                MaxShields = 6;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Cannon);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.YV666Table();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Crimson";

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
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
            }

        }
    }
}
