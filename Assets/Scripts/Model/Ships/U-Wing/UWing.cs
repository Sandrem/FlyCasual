﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Movement;
using ActionsList;
using Ship;
using RuleSets;

namespace Ship
{
    namespace UWing
    {
        public class UWing : GenericShip, IMovableWings, ISecondEditionShip
        {
            public WingsPositions CurrentWingsPosition { get; set; }

            public UWing() : base()
            {
                Type = "U-Wing";
                IconicPilots.Add(Faction.Rebel, typeof(BlueSquadronPathfinder));
                ShipBaseSize = BaseSize.Large;

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures/images/c/c5/MR_U-WING.png";

                ShipIconLetter = 'u';

                Firepower = 3;
                Agility = 1;
                MaxHull = 4;
                MaxShields = 4;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.System);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Torpedo);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Crew);

                PrintedActions.Add(new TargetLockAction());

                AssignTemporaryManeuvers();
                HotacManeuverTable = new AI.UWingTable();

                factions.Add(Faction.Rebel);
                faction = Faction.Rebel;

                SkinName = "White";

                CurrentWingsPosition = WingsPositions.Closed;

                SoundShotsPath = "Falcon-Fire";
                ShotsCount = 3;

                for (int i = 1; i < 4; i++)
                {
                    SoundFlyPaths.Add("Falcon-Fly" + i);
                }
            }

            private void AssignTemporaryManeuvers()
            {
                Maneuvers.Add("0.S.S", ManeuverColor.Red);
                Maneuvers.Add("1.L.B", ManeuverColor.Green);
                Maneuvers.Add("1.F.S", ManeuverColor.Green);
                Maneuvers.Add("1.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.L.T", ManeuverColor.White);
                Maneuvers.Add("2.L.B", ManeuverColor.Green);
                Maneuvers.Add("2.F.S", ManeuverColor.Green);
                Maneuvers.Add("2.R.B", ManeuverColor.Green);
                Maneuvers.Add("2.R.T", ManeuverColor.White);
                Maneuvers.Add("3.L.B", ManeuverColor.White);
                Maneuvers.Add("3.F.S", ManeuverColor.White);
                Maneuvers.Add("3.R.B", ManeuverColor.White);
                Maneuvers.Add("4.F.S", ManeuverColor.White);
            }

            public void AdaptShipToSecondEdition()
            {
                ShipBaseSize = BaseSize.Medium;

                Agility = 2;
                MaxHull = 5;
                MaxShields = 3;

                PrintedActions.Add(new CoordinateAction() { IsRed = true });

                IconicPilots[Faction.Rebel] = typeof(BenthicTwoTubes);
            }

        }
    }
}
