﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class TIEInterceptor : GenericShip, TIE
        {

            public TIEInterceptor() : base()
            {
                Type = "TIE Interceptor";
                IconicPilot = "Soontir Fel";

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/8/8e/MI_TIE-INTERCEPTOR.png";

                Firepower = 3;
                Agility = 3;
                MaxHull = 3;
                MaxShields = 0;

                PrintedActions.Add(new EvadeAction());
                PrintedActions.Add(new BarrelRollAction());
                PrintedActions.Add(new BoostAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEInterceptorTable();

                factions.Add(Faction.Imperial);
                faction = Faction.Imperial;

                SkinName = "Blue";

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
                Maneuvers.Add("1.L.B", ManeuverColor.None);
                Maneuvers.Add("1.F.S", ManeuverColor.None);
                Maneuvers.Add("1.R.B", ManeuverColor.None);
                Maneuvers.Add("1.R.T", ManeuverColor.White);
                Maneuvers.Add("1.F.R", ManeuverColor.None);
                Maneuvers.Add("2.L.T", ManeuverColor.Green);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.Green);
                Maneuvers.Add("2.F.R", ManeuverColor.None);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("3.F.R", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.Green);
                Maneuvers.Add("4.F.R", ManeuverColor.None);
                Maneuvers.Add("5.F.S", ManeuverColor.White);
                Maneuvers.Add("5.F.R", ManeuverColor.Red);
            }

        }
    }
}
