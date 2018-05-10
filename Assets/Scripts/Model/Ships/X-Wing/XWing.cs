﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using Ship;
using RuleSets;

namespace Ship
{
    namespace XWing
    {
        public class XWing : GenericShip, IMovableWings, ISecondEditionShip
        {
            public WingsPositions CurrentWingsPosition { get; set; }

            public XWing() : base()
            {
                Type = "X-Wing";
                IconicPilots.Add(Faction.Rebel, typeof(WedgeAntilles));

                ManeuversImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/3/3d/MR_T65-X-WING.png";

                ShipIconLetter = 'x';

                Firepower = 3;
                Agility = 2;
                MaxHull = 3;
                MaxShields = 2;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);

                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.XWingTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "Red";

                CurrentWingsPosition = WingsPositions.Opened;

                SoundShotsPath = "XWing-Laser";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("XWing-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("1.L.T", ManeuverColor.None);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("1.R.T", ManeuverColor.None);
                Maneuvers.Add("1.F.R", ManeuverColor.None);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.White);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.White);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("2.F.R", ManeuverColor.None);
                Maneuvers.Add("3.L.E", ManeuverColor.None);
                Maneuvers.Add("3.L.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("3.R.T", ManeuverColor.White);
                Maneuvers.Add("3.R.E", ManeuverColor.None);
                Maneuvers.Add("3.F.R", ManeuverColor.None);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
                Maneuvers.Add("4.F.R", ManeuverColor.Red);
                Maneuvers.Add("5.F.S", ManeuverColor.None);
                Maneuvers.Add("5.F.R", ManeuverColor.None);
            }

            public void AdaptShipToSecondEdition()
            {
                MaxHull = 5;
                PrintedActions.Add(new BarrelRollAction());

                Maneuvers["2.L.B"] = ManeuverColor.Green;
                Maneuvers["2.R.B"] = ManeuverColor.Green;
                Maneuvers["3.L.E"] = ManeuverColor.Red;
                Maneuvers["3.R.E"] = ManeuverColor.Red;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Modification);
            }
        }
    }
}
