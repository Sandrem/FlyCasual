﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;

namespace Ship
{
    namespace TIEPhantom
    {
        public class TIEPhantom : GenericShip, TIE
        {

            public TIEPhantom() : base()
            {
                Type = "TIE Phantom";

                ManeuversImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/8/85/MI_TIE-ADVANCED.png";

                Firepower = 4;
                Agility = 2;
                MaxHull = 2;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.TIEPhantomTable();

                factions.Add(Faction.Empire);
                faction = Faction.Empire;

                SkinName = "Gray";

                SoundShotsPath = "TIE-Fire";
                ShotsCount = 4;

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
                BuiltInActions.Add(new ActionsList.CloakAction());
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
                Maneuvers.Add("5.F.S", ManeuverColor.None);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

        }
    }
}
