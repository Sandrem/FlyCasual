using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace M3AScyk
    {
        public class M3AScyk : GenericShip
        {

            public M3AScyk() : base()
            {
                Type = "M3-A Interceptor";
                IconicPilot = "Tansarii Point Veteran";

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/4/48/MS_M3-A-INTERCEPTOR.png";

                Firepower = 2;
                Agility = 3;
                MaxHull = 2;
                MaxShields = 1;

                PrintedActions.Add(new EvadeAction());
                PrintedActions.Add(new BarrelRollAction());
                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.M3AScykTable();

                factions.Add(Faction.Scum);
                faction = Faction.Scum;

                SkinName = "Inaldra";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
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
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.F.R", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("5.F.S", ManeuverColor.White);
                Maneuvers.Add("5.F.R", ManeuverColor.Red);
            }

        }
    }
}
