using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace Kihraxz
    {
        public class Kihraxz : GenericShip
        {

            public Kihraxz() : base()
            {
                Type = "Kihraxz Fighter";

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/d/d8/MS_KIHRAXZ-FIGHTER.png";

                Firepower = 3;
                Agility = 2;
                MaxHull = 4;
                MaxShields = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.KihraxzTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Hutt Cartel";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
                
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.White);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.None);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("1.R.T", ManeuverColor.White);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.R", ManeuverColor.Red);
            }

        }
    }
}
