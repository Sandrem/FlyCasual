using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace TIEStriker
    {
        public class TIEStriker : GenericShip, TIE
        {

            public TIEStriker() : base()
            {
                Type = "TIE Striker";

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/9/9b/MI_TIE-STRIKER.png";

                Firepower = 3;
                Agility = 2;
                MaxHull = 4;
                MaxShields = 0;

                PrintedActions.Add(new EvadeAction());
                PrintedActions.Add(new BarrelRollAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = null;

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Gray";

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
                Maneuvers.Add("2.L.R", ManeuverColor.Red);
                Maneuvers.Add("2.F.R", ManeuverColor.Red);
                Maneuvers.Add("2.R.R", ManeuverColor.Red);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
            }

        }
    }
}
