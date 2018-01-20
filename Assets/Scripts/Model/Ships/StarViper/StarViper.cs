using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace StarViper
    {
        public class StarViper : GenericShip
        {

            public StarViper() : base()
            {
                Type = "StarViper";
                IconicPilot = "Thweek";

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/b/bd/MS_STARVIPER.png";

                Firepower = 3;
                Agility = 3;
                MaxHull = 4;
                MaxShields = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new BarrelRollAction());
                PrintedActions.Add(new BoostAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.StarviperTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Black Sun Enforcer";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.White);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("1.R.T", ManeuverColor.White);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.L.R", ManeuverColor.Red);
                Maneuvers.Add("3.R.R", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
            }

        }
    }
}
