﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;

namespace Ship
{
    namespace SheathipedeShuttle
    {
        public class SheathipedeShuttle : GenericShip
        {

            public SheathipedeShuttle() : base()
            {
                Type = "Sheathipede-class Shuttle";
                IconicPilot = "AP-5";

                ShipBaseArcsType = Arcs.BaseArcsType.ArcRear;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/3/30/Maneuver_Card_-_Sheathipede-class_Shuttle.png";

                Firepower = 2;
                Agility = 2;
                MaxHull = 4;
                MaxShields = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                PrintedActions.Add(new TargetLockAction());
                PrintedActions.Add(new CoordinateAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.SheathipedeShuttleTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Phantom II";

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 2;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
                
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.B", ManeuverColor.White);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.White);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.T", ManeuverColor.Red);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.Green);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.Red);
                Maneuvers.Add("3.F.R", ManeuverColor.Red);
                Maneuvers.Add("4.F.S", ManeuverColor.Red);
            }

        }
    }
}
