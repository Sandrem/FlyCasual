﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace Ship
{
    namespace TIEFighter
    {
        public class TIEFighter : GenericShip, TIE
        {

            public TIEFighter() : base()
            {
                Type = "TIE Fighter";

                ManeuversImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/b/b6/TIE_Fighter_Move.png";

                Firepower = 2;
                Agility = 3;
                MaxHull = 3;
                MaxShields = 0;

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEFighterTable();

                factions.Add(Faction.Empire);
                factions.Add(Faction.Rebels);
                faction = Faction.Empire;

                SkinName = "Gray";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 2;

                for (int i = 1; i < 8; i++)
                {
                    SoundFlyPaths.Add("TIE-Fly" + i);
                }
            }

            public override void InitializeShip()
            {
                base.InitializeShip();
                BuiltInActions.Add(new ActionsList.EvadeAction());
                BuiltInActions.Add(new ActionsList.BarrelRollAction());
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.White);
                Maneuvers.Add("1.L.B", ManeuverColor.None);
                Maneuvers.Add("1.F.S", ManeuverColor.None);
                Maneuvers.Add("1.R.B", ManeuverColor.None);
                Maneuvers.Add("1.R.T", ManeuverColor.White);
                Maneuvers.Add("1.F.R", ManeuverColor.None);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("2.F.R", ManeuverColor.None);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("3.F.R", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.White);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

        }
    }
}
