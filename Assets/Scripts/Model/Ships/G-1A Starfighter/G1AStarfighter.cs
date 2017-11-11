using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace G1AStarfighter
    {
        public class G1AStarfighter : GenericShip
        {

            public G1AStarfighter() : base()
            {
                Type = "G-1A Starfighter";

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/6/67/MS_G-1A-STARFIGHTER.png";

                Firepower = 3;
                Agility = 1;
                MaxHull = 4;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new EvadeAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = null;

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "G-1A Starfighter";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
                
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.Red);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("1.R.T", ManeuverColor.Red);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.Red);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.Red);
                Maneuvers.Add("3.F.R", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
            }

        }
    }
}
